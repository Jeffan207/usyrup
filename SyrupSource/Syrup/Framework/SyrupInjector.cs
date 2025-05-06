using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Syrup.Framework.Exceptions;
using Syrup.Framework.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using Binder = Syrup.Framework.Declarative.Binder;

namespace Syrup.Framework {
    /// <summary>
    ///     Injects syrup directly into your veins (and by syrup I mean your dependencies and by veins I
    ///     mean your code).
    /// </summary>
    public class SyrupInjector {
        private readonly Dictionary<NamedDependency, HashSet<NamedDependency>> _paramOfDependencies = new();
        private readonly Dictionary<NamedDependency, DependencyInfo> _dependencySources = new();
        private readonly Dictionary<NamedDependency, object> _fulfilledDependencies = new();
        private Dictionary<NamedDependency, int> _indegreesForType;
        private readonly bool _verboseLogging;

        public SyrupInjector(params ISyrupModule[] modules) {
            _verboseLogging = false;

            AddSyrupModules(modules);
        }

        public SyrupInjector(
            SyrupInjectorOptions syrupInjectorOptions, params ISyrupModule[] modules
        ) {
            _verboseLogging = syrupInjectorOptions.VerboseLogging;

            AddSyrupModules(modules);
        }

        #region Public Methods

        /// <summary>
        ///     Retrieves an instance of object of type T from the dependency graph.
        ///     Objects will be built as needed.
        /// </summary>
        /// <returns>The object of type T build from the dependency graph</returns>
        public T GetInstance<T>() => GetInstance<T>(null);

        /// <summary>
        ///     Returns a named dependency from the dependency graph. Passing null is equivalent to requesting
        ///     an
        ///     unnamed dependency of that type.
        /// </summary>
        /// <typeparam name="T">The name of the type if explicitly given one with the [Named] attribute</typeparam>
        /// <returns></returns>
        public T GetInstance<T>(string name) =>
            (T) BuildDependency(new NamedDependency(name, typeof(T)));

        /// <summary>
        ///     Injects dependencies into the provided object by invoking any inject related methods.
        /// </summary>
        /// <param name="objectToInject">The object to be injected</param>
        public void Inject<T>(T objectToInject) {
            var injectableFields = SyrupUtils.GetInjectableFieldsFromType(objectToInject.GetType());
            var injectableMethods =
                SyrupUtils.GetInjectableMethodsFromType(objectToInject.GetType());
            InjectObject(objectToInject, injectableFields, injectableMethods);
        }

        #endregion

        #region Internal Methods

        internal void AddSyrupModules(params ISyrupModule[] modules) {
            _indegreesForType = new Dictionary<NamedDependency, int>();

            //Fetch all provider methods declared in all provided modules
            foreach (var module in modules) {
                var providerMethods = module.GetType().GetMethods()
                    .Where(x =>
                        x.GetCustomAttributes(typeof(Provides), false).FirstOrDefault() != null);

                foreach (var methodInfo in providerMethods) {
                    if (IsLazyWrapped(methodInfo.ReturnType)) {
                        throw new InvalidProvidedDependencyException(
                            $"A provider is trying to provide a Lazy type explicitly for '{methodInfo.ReturnType.FullName}', try pushing the Lazy type down onto the consuming class instead");
                    }

                    var dependencyName = methodInfo.GetCustomAttribute<Named>();
                    var name = dependencyName?.name;
                    var namedDependency = new NamedDependency(name, methodInfo.ReturnType);
                    var isSingleton = methodInfo.GetCustomAttribute<Singleton>() != null;

                    //This is a convenient place to check if multiple providers for a single type have been declared
                    if (_indegreesForType.ContainsKey(namedDependency)) {
                        throw new DuplicateProviderException
                            ($"A provider for the specified dependency '{namedDependency}' already exists!");
                    }

                    HashSet<NamedDependency> uniqueParameters = new();
                    foreach (var param in methodInfo.GetParameters()) {
                        uniqueParameters.Add(GetNamedDependencyForParam(param));
                    }

                    _indegreesForType.Add(namedDependency, uniqueParameters.Count);

                    DependencyInfo dependencyInfo = new() {
                        DependencySource = DependencySource.PROVIDER,
                        ProviderMethod = methodInfo,
                        ReferenceObject = module,
                        Type = methodInfo.ReturnType,
                        IsSingleton = isSingleton
                    };

                    _dependencySources[namedDependency] = dependencyInfo;
                    AddDependenciesForParam(namedDependency, uniqueParameters.ToList());
                }
            }

            // Process declarative bindings from Configure()
            foreach (var module in modules) {
                var binder = new Binder();
                module.Configure(binder);

                foreach (var binding in binder.GetBindings()) {
                    var namedDependency = new NamedDependency(binding.Name, binding.BoundService);

                    if (_dependencySources.ContainsKey(namedDependency) &&
                        _dependencySources[namedDependency].DependencySource ==
                        DependencySource.PROVIDER) {
                        if (_verboseLogging) {
                            Debug.Log(
                                $"Declarative binding for '{namedDependency}' skipped as a Provider already exists for it.");
                        }

                        continue;
                    }

                    if (_dependencySources.ContainsKey(namedDependency) &&
                        _dependencySources[namedDependency].DependencySource ==
                        DependencySource.DECLARATIVE) {
                        throw new DuplicateProviderException(
                            $"A declarative binding for the specified dependency '{namedDependency}' has already been registered!");
                    }

                    DependencyInfo dependencyInfo = new() {
                        DependencySource = DependencySource.DECLARATIVE,
                        Type = binding.BoundService,
                        IsSingleton = binding.IsSingleton,
                        ImplementationType = binding.ImplementationType,
                        Instance = binding.Instance
                    };

                    int requiredParamsCount;
                    var uniqueParameters = new HashSet<NamedDependency>();

                    if (binding.Instance != null) {
                        requiredParamsCount = 0;
                    } else if (binding.ImplementationType != null) {
                        var implConstructor = SelectConstructorForType(binding.ImplementationType);
                        dependencyInfo.Constructor = implConstructor;

                        if (implConstructor != null) {
                            foreach (var param in implConstructor.GetParameters()) {
                                uniqueParameters.Add(GetNamedDependencyForParam(param));
                            }
                        } else if (!binding.ImplementationType.IsValueType &&
                                   !IsStatic(binding.ImplementationType) &&
                                   !binding.ImplementationType.IsAbstract &&
                                   !binding.ImplementationType.IsInterface &&
                                   binding.ImplementationType
                                       .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                                       .Any(c => c.GetParameters().Length == 0)) {
                            // Handled by SelectConstructorForType
                        } else if (binding.ImplementationType.IsValueType ||
                                   (IsStatic(binding.ImplementationType) && binding
                                       .ImplementationType
                                       .GetConstructors(BindingFlags.Public |
                                                        BindingFlags.Instance | BindingFlags.Static)
                                       .All(c => c.GetParameters().Length > 0 || !c.IsPublic))) {
                            // Value types or unconstructable static classes
                        }

                        var injectableFields =
                            SyrupUtils.GetInjectableFieldsFromType(binding.ImplementationType);
                        foreach (var injectableField in injectableFields) {
                            uniqueParameters.Add(GetNamedDependencyForField(injectableField));
                        }

                        dependencyInfo.InjectableFields = injectableFields;

                        var injectableMethods =
                            SyrupUtils.GetInjectableMethodsFromType(binding.ImplementationType);
                        foreach (var injectableMethod in injectableMethods) {
                            foreach (var param in injectableMethod.GetParameters()) {
                                uniqueParameters.Add(GetNamedDependencyForParam(param));
                            }
                        }

                        dependencyInfo.InjectableMethods = injectableMethods;

                        requiredParamsCount = uniqueParameters.Count;
                    } else {
                        throw new InvalidOperationException(
                            $"Declarative binding for '{namedDependency}' is incomplete. Must call To<TImplementation>() or ToInstance().");
                    }

                    _indegreesForType[namedDependency] = requiredParamsCount;
                    _dependencySources[namedDependency] = dependencyInfo;
                    AddDependenciesForParam(namedDependency, uniqueParameters.ToList());
                }
            }

            //Fetch all injectable constructors
            var injectedConstructors = AppDomain.CurrentDomain
                .GetAssemblies() // Returns all currently loaded assemblies
                .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
                .Where(x =>
                    x.IsClass &&
                    !x.IsAbstract) // Exclude abstract classes as well since they cannot be initiated
                .SelectMany(x => x.GetConstructors())
                .Where(x => x.GetCustomAttributes(typeof(Inject), false).FirstOrDefault() != null);

            foreach (var constructor in injectedConstructors) {
                //You cannot name a constructor in constructor injection, so treat the name as null
                var constructorDeclaringType = constructor.DeclaringType;
                var namedDependency = new NamedDependency(null, constructorDeclaringType);
                var isSingleton = constructorDeclaringType.GetCustomAttribute<Singleton>() != null;

                if (_indegreesForType.ContainsKey(namedDependency)) {
                    //Providers take precedence over constructors for injection.
                    //If both are provided, ignore the constructor.
                    continue;
                }

                HashSet<NamedDependency> uniqueParameters = new();
                foreach (var param in constructor.GetParameters()) {
                    uniqueParameters.Add(GetNamedDependencyForParam(param));
                }

                var injectableFields =
                    SyrupUtils.GetInjectableFieldsFromType(constructorDeclaringType);
                foreach (var injectableField in injectableFields) {
                    uniqueParameters.Add(GetNamedDependencyForField(injectableField));
                }

                var injectableMethods =
                    SyrupUtils.GetInjectableMethodsFromType(constructorDeclaringType);
                foreach (var injectableMethod in injectableMethods) {
                    foreach (var param in injectableMethod.GetParameters()) {
                        uniqueParameters.Add(GetNamedDependencyForParam(param));
                    }
                }

                _indegreesForType.Add(namedDependency, uniqueParameters.Count());

                DependencyInfo dependencyInfo = new() {
                    DependencySource = DependencySource.CONSTRUCTOR,
                    Constructor = constructor,
                    Type = constructorDeclaringType,
                    IsSingleton = isSingleton,
                    InjectableMethods = injectableMethods,
                    InjectableFields = injectableFields
                };

                _dependencySources[namedDependency] = dependencyInfo;
                AddDependenciesForParam(namedDependency, uniqueParameters.ToList());
            }

            ValidateDependencyGraph();
        }

        /// <summary>
        ///     Injects all methods attached to MBs within all scenes that have methods annotated with the
        ///     [Inject]
        ///     attribute. In order to prevent duplicate injections this should only be called from within the
        ///     SyrupComponent. Method injection ignores the return type of the injected method, so don't rely
        ///     on it
        ///     to fulfill dependencies for the dependency graph inside the injector.
        /// </summary>
        internal void InjectAllGameObjects() {
            List<InjectableMonoBehaviour> injectableMonoBehaviours = new();

            for (var i = 0; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded) {
                    SyrupUtils.GetInjectableMonoBehaviours(scene, injectableMonoBehaviours);
                }
            }

            InjectGameObjects(injectableMonoBehaviours);
        }

        /// <summary>
        ///     Injects all methods in a specified scene that have methods annotated with the [Inject]
        ///     attribute.
        ///     Use this if you plan to dynamically add scenes that will need to be injected outside the games
        ///     initial setup steps.
        /// </summary>
        internal void InjectGameObjectsInScene(Scene scene) {
            List<InjectableMonoBehaviour> injectableMonoBehaviours = new();
            SyrupUtils.GetInjectableMonoBehaviours(scene, injectableMonoBehaviours);
            InjectGameObjects(injectableMonoBehaviours);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Goes through all params for a given dependency and marks that the dependency is a type in the
        ///     graph
        ///     that requires it.
        /// </summary>
        private void AddDependenciesForParam(
            NamedDependency namedDependency, List<NamedDependency> namedParameters
        ) {
            foreach (var namedParam in namedParameters) {
                var dependentTypes =
                    _paramOfDependencies.TryGetValue(namedParam, out var dependency)
                        ? dependency
                        : new HashSet<NamedDependency>();
                dependentTypes.Add(namedDependency);
                _paramOfDependencies[namedParam] = dependentTypes;
            }
        }

        /// <summary>
        ///     Uses Kahn's algorithm to topologically sort the dependency graph so we can validate it
        ///     from bottom to top. Objects are not actually built in this phase, only validated.
        ///     I knew that my CS education would come in handy :) (jk I actually learned this one through
        ///     leetcode...)
        /// </summary>
        private void ValidateDependencyGraph() {
            var queue = new Queue<NamedDependency>();
            var currentIndegrees = new Dictionary<NamedDependency, int>(_indegreesForType);

            foreach (var key in currentIndegrees.Keys.Where(key => currentIndegrees[key] == 0)) {
                queue.Enqueue(key);
            }

            var visitedCount = 0;
            while (queue.Count > 0) {
                var namedDependency = queue.Dequeue();
                visitedCount++;

                if (!_paramOfDependencies.TryGetValue(namedDependency, out var dependentTypes)) {
                    continue;
                }

                foreach (var dependentType in dependentTypes) {
                    if (!currentIndegrees.TryGetValue(dependentType, out var indegrees)) {
                        continue;
                    }

                    indegrees--;
                    currentIndegrees[dependentType] = indegrees;

                    if (indegrees == 0) {
                        queue.Enqueue(dependentType);
                    }
                }
            }

            if (visitedCount < currentIndegrees.Count) {
                var missingDependenciesCycle = currentIndegrees.Keys
                    .Where(namedDependency => currentIndegrees[namedDependency] > 0)
                    .Where(IsMeaningfulDependency)
                    .Aggregate("",
                        (current, namedDependency) => current +
                                                      ConstructMissingDependencyStringForType(
                                                          namedDependency));
                if (!string.IsNullOrEmpty(missingDependenciesCycle)) {
                    throw new CircularDependencyException(
                        $"Circular dependency detected or missing dependencies preventing graph completion. problematic dependencies:\n{missingDependenciesCycle}");
                }
            }

            var missingDependencies = "";
            var incompleteGraph = false;
            foreach (var namedDependency in _indegreesForType.Keys
                         .Where(namedDependency => currentIndegrees.ContainsKey(namedDependency) &&
                                                   currentIndegrees[namedDependency] > 0)
                         .Where(IsMeaningfulDependency)) {
                missingDependencies += ConstructMissingDependencyStringForType(namedDependency);
                incompleteGraph = true;
            }

            if (incompleteGraph) {
                throw new MissingDependencyException(
                    $"Incomplete dependency graph. The following dependencies are missing from the completed graph:\n{missingDependencies}");
            }
        }

        /// <summary>
        ///     Given a named dependency go through and iterate through the dependency's
        ///     required dependencies and build them, ultimately returning the requested
        ///     dependency.
        /// </summary>
        /// <param name="namedDependency">The NamedDependency object to build</param>
        /// <returns>The requested dependency or its LazyObject wrapped container</returns>
        private object BuildDependency(NamedDependency namedDependency) {
            var dependencyToBuild = namedDependency;
            var isLazy = IsLazyWrapped(namedDependency.Type);
            if (isLazy) {
                var containedType = GetContainedType(namedDependency.Type);
                dependencyToBuild = new NamedDependency(namedDependency.Name, containedType);
                if (_verboseLogging) {
                    Debug.Log($"Requested lazy instance of type: {dependencyToBuild}");
                }
            }

            if (!_dependencySources.TryGetValue(dependencyToBuild, out var dependencyInfo)) {
                throw new MissingDependencyException(
                    $"'{dependencyToBuild}' is not a provided dependency!");
            }

            if (dependencyInfo.IsSingleton &&
                _fulfilledDependencies.TryGetValue(dependencyToBuild, out var buildDependency)) {
                if (_verboseLogging) {
                    Debug.Log($"Provide singleton: {dependencyToBuild}");
                }

                return buildDependency;
            }

            // Let's also check the Lazy version for this dependency.
            // Lazy containers should be singletons if the underlying type is a singleton
            // (Note: we pass in the original namedDependency param since it's already Lazy!)
            if (isLazy && dependencyInfo.IsSingleton &&
                _fulfilledDependencies.TryGetValue(namedDependency, out var dependency1)) {
                if (_verboseLogging) {
                    Debug.Log($"Provide lazy singleton: {namedDependency}");
                }

                return dependency1;
            }

            if (_verboseLogging) {
                Debug.Log($"Constructing object: {dependencyToBuild}");
            }

            // We don't want to build the graph for lazy dependencies during the injection
            // phase, so for these dependencies we build a lazy container for them and
            // return that container directly.
            if (isLazy) {
                // Since we cannot just create arbitrary generic types Lazy<T> instances at runtime
                // we need to use reflection to create them instead.
                var lazyDependency =
                    Activator.CreateInstance(namedDependency.Type, namedDependency.Name, this);

                if (dependencyInfo.IsSingleton) {
                    _fulfilledDependencies.Add(namedDependency, lazyDependency);
                }

                return lazyDependency;
            }

            object dependency;
            switch (dependencyInfo.DependencySource) {
                case DependencySource.PROVIDER: {
                    var method = dependencyInfo.ProviderMethod;
                    var parameters = GetMethodParameters(method);
                    dependency = method.Invoke(dependencyInfo.ReferenceObject, parameters);
                    break;
                }
                case DependencySource.CONSTRUCTOR: {
                    var constructor = dependencyInfo.Constructor;
                    var parameters = GetConstructorParameters(constructor);
                    dependency = constructor.Invoke(parameters);
                    InjectObject(dependency, dependencyInfo.InjectableFields,
                        dependencyInfo.InjectableMethods);
                    break;
                }
                case DependencySource.DECLARATIVE when dependencyInfo.Instance != null:
                    dependency = dependencyInfo.Instance;
                    break;
                case DependencySource.DECLARATIVE when dependencyInfo.ImplementationType != null: {
                    var constructorToUse = dependencyInfo.Constructor;
                    if (constructorToUse != null) {
                        var parameters = GetConstructorParameters(constructorToUse);
                        dependency = constructorToUse.Invoke(parameters);
                    } else if (dependencyInfo.ImplementationType.IsValueType) {
                        dependency = Activator.CreateInstance(dependencyInfo.ImplementationType);
                    } else {
                        throw new MissingMemberException(
                            $"Cannot create instance of '{dependencyInfo.ImplementationType.FullName}' for declarative binding '{dependencyToBuild}'. No suitable constructor found or specified during registration.");
                    }

                    InjectObject(dependency, dependencyInfo.InjectableFields,
                        dependencyInfo.InjectableMethods);
                    break;
                }
                case DependencySource.DECLARATIVE:
                    throw new InvalidOperationException(
                        $"Invalid declarative binding for '{dependencyToBuild}'. No instance or implementation type.");
                default:
                    throw new UnknownDependencySourceException(
                        $"Unknown DependencySource: '{dependencyInfo.DependencySource}', cannot fulfill dependency!");
            }

            if (dependencyInfo.IsSingleton) {
                _fulfilledDependencies.Add(dependencyToBuild, dependency);
            }

            return dependency;
        }

        private object[] GetMethodParameters(MethodInfo method) {
            var paramIndex = 0;
            var parameters = new object[method.GetParameters().Length];
            foreach (var parameterInfo in method.GetParameters()) {
                parameters[paramIndex] =
                    BuildDependency(GetNamedDependencyForParamInjection(parameterInfo));
                paramIndex++;
            }

            return parameters;
        }

        private object[] GetConstructorParameters(ConstructorInfo constructor) {
            var paramIndex = 0;
            var parameters = new object[constructor.GetParameters().Length];
            foreach (var parameterInfo in constructor.GetParameters()) {
                parameters[paramIndex] =
                    BuildDependency(GetNamedDependencyForParamInjection(parameterInfo));
                paramIndex++;
            }

            return parameters;
        }

        /// <summary>
        ///     A meaningful dependency is one that the module needs to provide because it directly rolls up
        ///     into a Provider annotated method in the module itself. Some constructor injected objects might
        ///     not
        ///     be provided by the module, so we need to validate if they should fail graph validation or not
        /// </summary>
        private bool IsMeaningfulDependency(NamedDependency namedDependency) {
            if (!_dependencySources.TryGetValue(namedDependency, out var source)) {
                return false;
            }

            var dependencySource = source.DependencySource;
            return dependencySource switch {
                DependencySource.PROVIDER or DependencySource.DECLARATIVE => true,
                DependencySource.CONSTRUCTOR => _paramOfDependencies.TryGetValue(namedDependency,
                    out var dependency) && dependency.Any(IsMeaningfulDependency),
                _ => false
            };
        }

        private string ConstructMissingDependencyStringForType(NamedDependency namedDependency) {
            if (!_dependencySources.TryGetValue(namedDependency, out var dependencyInfo)) {
                return $"'{namedDependency}' is not a known dependency.\n";
            }

            var parameters = new List<NamedDependency>();

            switch (dependencyInfo.DependencySource) {
                case DependencySource.PROVIDER:
                    parameters = dependencyInfo.ProviderMethod.GetParameters()
                        .Select(GetNamedDependencyForParam)
                        .ToList();
                    break;
                case DependencySource.CONSTRUCTOR: {
                    parameters.AddRange(
                        dependencyInfo.Constructor.GetParameters()
                            .Select(GetNamedDependencyForParam)
                            .ToList());
                    if (dependencyInfo.InjectableFields != null) {
                        parameters.AddRange(
                            dependencyInfo.InjectableFields.Select(GetNamedDependencyForField));
                    }

                    if (dependencyInfo.InjectableMethods != null) {
                        foreach (var injectableMethod in dependencyInfo.InjectableMethods) {
                            parameters.AddRange(
                                injectableMethod.GetParameters()
                                    .Select(GetNamedDependencyForParam)
                                    .ToList());
                        }
                    }

                    break;
                }
                case DependencySource.DECLARATIVE when dependencyInfo.Instance != null:
                    // No params for instance
                    break;
                case DependencySource.DECLARATIVE when dependencyInfo.ImplementationType != null: {
                    if (dependencyInfo.Constructor != null) {
                        parameters.AddRange(
                            dependencyInfo.Constructor.GetParameters()
                                .Select(GetNamedDependencyForParam)
                                .ToList());
                    }

                    if (dependencyInfo.InjectableFields != null) {
                        parameters.AddRange(
                            dependencyInfo.InjectableFields.Select(GetNamedDependencyForField));
                    }

                    if (dependencyInfo.InjectableMethods != null) {
                        foreach (var injectableMethod in dependencyInfo.InjectableMethods) {
                            parameters.AddRange(
                                injectableMethod.GetParameters()
                                    .Select(GetNamedDependencyForParam)
                                    .ToList());
                        }
                    }

                    break;
                }
                case DependencySource.DECLARATIVE:
                    return $"'{namedDependency}' (Declarative) is misconfigured.\n";
                default:
                    return
                        $"'{namedDependency}' has an unknown dependency source '{dependencyInfo.DependencySource}'.\n";
            }

            var missingParams = (from namedParam in parameters
                where !_dependencySources.ContainsKey(namedParam) ||
                      (_indegreesForType.ContainsKey(namedParam) &&
                       _indegreesForType[namedParam] > 0 && IsMeaningfulDependency(namedParam))
                select namedParam.ToString()).ToList();

            return missingParams.Any()
                ? $"'{namedDependency}' is missing the following dependencies: '{string.Join(", ", missingParams)}'\n"
                : "";
        }

        private void InjectObject<T>(
            T objectToInject, FieldInfo[] injectableFields, MethodInfo[] injectableMethods
        ) {
            //We're making the assumption that the injectable fields/methods are ordered from base class
            //to deriving class (they should be) but we're assuming it too.
            foreach (var injectableField in injectableFields) {
                if (_verboseLogging) {
                    Debug.Log(
                        $"Injecting (Field) [{injectableField.FieldType}] into [{objectToInject.GetType()}]");
                }

                injectableField.SetValue(objectToInject,
                    BuildDependency(GetNamedDependencyForFieldInjection(injectableField)));
            }

            foreach (var injectableMethod in injectableMethods) {
                var parameters = GetMethodParameters(injectableMethod);
                if (_verboseLogging) {
                    var methodParams = string.Join(",",
                        parameters.Select(parameter => parameter.GetType().ToString()).ToArray());
                    Debug.Log(
                        $"Injecting (Method Params) [{methodParams}] into [{objectToInject.GetType()}]");
                }

                injectableMethod.Invoke(objectToInject, parameters);
            }
        }

        private void InjectGameObjects(List<InjectableMonoBehaviour> injectableMonoBehaviours) {
            foreach (var injectableMb in injectableMonoBehaviours) {
                InjectObject(injectableMb.mb, injectableMb.fields, injectableMb.methods);
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        ///     This method (and it's sister field method) should be used for
        ///     building the dependency hierarchy. For that process we want to
        ///     discard any containers so we can build the underlying types.
        /// </summary>
        private static NamedDependency GetNamedDependencyForParam(ParameterInfo param) {
            var dependencyName = param.GetCustomAttribute<Named>();
            var name = dependencyName?.name;
            var paramType = GetContainedType(param.ParameterType);
            return new NamedDependency(name, paramType);
        }

        private static NamedDependency GetNamedDependencyForField(FieldInfo field) {
            var dependencyName = field.GetCustomAttribute<Named>();
            var name = dependencyName?.name;
            var fieldType = GetContainedType(field.FieldType);
            return new NamedDependency(name, fieldType);
        }

        private static Type GetContainedType(Type type) =>
            IsLazyWrapped(type) ? type.GetGenericArguments()[0] : type;

        private static bool IsLazyWrapped(Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LazyObject<>);

        /// <summary>
        ///     This method (and it's sister field method) should be used for
        ///     injecting fields/methods into injectable objects directly. We
        ///     want the full types for those injections.
        /// </summary>
        private static NamedDependency GetNamedDependencyForParamInjection(ParameterInfo param) =>
            new(param.GetCustomAttribute<Named>()?.name, param.ParameterType);

        private static NamedDependency GetNamedDependencyForFieldInjection(FieldInfo field) =>
            new(field.GetCustomAttribute<Named>()?.name, field.FieldType);

        private static ConstructorInfo SelectConstructorForType(Type type) {
            if (type == null || type.IsAbstract || type.IsInterface || IsStatic(type)) {
                return null;
            }

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (!constructors.Any()) {
                return null;
            }

            var injectAnnotatedConstructors = constructors
                .Where(c => c.IsDefined(typeof(Inject), false))
                .ToList();

            switch (injectAnnotatedConstructors.Count) {
                case > 1:
                    throw new InvalidOperationException(
                        $"Type '{type.FullName}' has multiple constructors annotated with [Inject].");
                case 1:
                    return injectAnnotatedConstructors.First();
            }

            if (constructors.Length == 1) {
                return constructors[0];
            }

            var parameterlessConstructor =
                constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
            if (parameterlessConstructor != null) {
                return parameterlessConstructor;
            }

            if (constructors.Length > 1) {
                throw new InvalidOperationException(
                    $"Type {type.FullName} has multiple public constructors and none are marked with [Inject] or are parameterless. Cannot determine which to use for declarative binding.");
            }

            return null;
        }

        private static bool IsStatic(Type type) => type.IsAbstract && type.IsSealed;

        #endregion
    }
}
