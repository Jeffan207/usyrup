using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Syrup.Framework.Declarative;
using Syrup.Framework.Exceptions;
using Syrup.Framework.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using Binder = Syrup.Framework.Declarative.Binder;

namespace Syrup.Framework {

    /// <summary>
    /// Injects syrup directly into your veins (and by syrup I mean your dependencies and by veins I mean your code).
    /// </summary>
    public class SyrupInjector {

        //Used in Kahn's algorithm for graph validation
        private Dictionary<NamedDependency, int> indegreesForType;

        //Information regarding where the source of a dependency of a particular type
        private readonly Dictionary<NamedDependency, DependencyInfo> dependencySources;

        //Given a param, map to all types that need it
        private readonly Dictionary<NamedDependency, HashSet<NamedDependency>> paramOfDependencies;

        private bool verboseLogging = false;

        //Dependencies that have been fully constructed
        private readonly Dictionary<NamedDependency, object> fulfilledDependencies;

        public SyrupInjector(params ISyrupModule[] modules) {
            dependencySources = new Dictionary<NamedDependency, DependencyInfo>();
            paramOfDependencies = new Dictionary<NamedDependency, HashSet<NamedDependency>>();
            fulfilledDependencies = new Dictionary<NamedDependency, object>();

            AddSyrupModules(modules);
        }

        public SyrupInjector(SyrupInjectorOptions syrupInjectorOptions, params ISyrupModule[] modules) {
            verboseLogging = syrupInjectorOptions.VerboseLogging;

            dependencySources = new Dictionary<NamedDependency, DependencyInfo>();
            paramOfDependencies = new Dictionary<NamedDependency, HashSet<NamedDependency>>();
            fulfilledDependencies = new Dictionary<NamedDependency, object>();

            AddSyrupModules(modules);
        }

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
            FieldInfo[] injectableFields = SyrupUtils.GetInjectableFieldsFromType(objectToInject.GetType());
            MethodInfo[] injectableMethods =
                SyrupUtils.GetInjectableMethodsFromType(objectToInject.GetType());
            InjectObject(objectToInject, injectableFields, injectableMethods);
        }

        internal void AddSyrupModules(params ISyrupModule[] modules) {
            indegreesForType = new Dictionary<NamedDependency, int>();

            //Fetch all provider methods declared in all provided modules
            foreach (ISyrupModule module in modules) {
                IEnumerable<MethodInfo> providerMethods = module.GetType().GetMethods()
                    .Where(x => x.GetCustomAttributes(typeof(Provides), false).FirstOrDefault() != null);

                foreach (MethodInfo methodInfo in providerMethods) {
                    if (IsLazyWrapped(methodInfo.ReturnType)) {
                        throw new InvalidProvidedDependencyException(
                            $"A provider is trying to provide a Lazy type explicitly for '{methodInfo.ReturnType.FullName}', try pushing the Lazy type down onto the consuming class instead");
                    }

                    Named dependencyName = methodInfo.GetCustomAttribute<Named>();
                    string name = dependencyName != null ? dependencyName.name : null;
                    NamedDependency namedDependency = new NamedDependency(name, methodInfo.ReturnType);
                    bool isSingleton = methodInfo.GetCustomAttribute<Singleton>() != null;

                    //This is a convenient place to check if multiple providers for a single type have been declared
                    if (indegreesForType.ContainsKey(namedDependency)) {
                        throw new DuplicateProviderException
                            ($"A provider for the specified dependency '{namedDependency}' already exists!");
                    }

                    HashSet<NamedDependency> uniqueParameters = new();
                    foreach (ParameterInfo param in methodInfo.GetParameters()) {
                        uniqueParameters.Add(GetNamedDependencyForParam(param));
                    }
                    indegreesForType.Add(namedDependency, uniqueParameters.Count());

                    DependencyInfo dependencyInfo = new() {
                        DependencySource = DependencySource.PROVIDER,
                        ProviderMethod = methodInfo,
                        ReferenceObject = module,
                        Type = methodInfo.ReturnType,
                        IsSingleton = isSingleton
                    };

                    dependencySources[namedDependency] = dependencyInfo;
                    AddDependenciesForParam(namedDependency, uniqueParameters.ToList());
                }
            }

            // Process declarative bindings from Configure()
            foreach (ISyrupModule module in modules) {
                Binder binder = new Binder();
                module.Configure(binder);

                foreach (Binding binding in binder.GetBindings()) {
                    NamedDependency namedDependency = new NamedDependency(binding.Name, binding.BoundService);

                    var dependencySource = dependencySources[namedDependency].DependencySource;
                    if (dependencySources.ContainsKey(namedDependency) &&
                        (dependencySource == DependencySource.PROVIDER || dependencySource == DependencySource.DECLARATIVE)) {
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
                    HashSet<NamedDependency> uniqueParameters = new HashSet<NamedDependency>();

                    if (binding.Instance != null) {
                        requiredParamsCount = 0;
                    } else if (binding.ImplementationType != null) {
                        ConstructorInfo implConstructor = SelectConstructorForType(binding.ImplementationType);
                        dependencyInfo.Constructor = implConstructor;

                        if (implConstructor != null) {
                            foreach (ParameterInfo param in implConstructor.GetParameters()) {
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

                        FieldInfo[] injectableFields =
                            SyrupUtils.GetInjectableFieldsFromType(binding.ImplementationType);
                        foreach (FieldInfo injectableField in injectableFields) {
                            uniqueParameters.Add(GetNamedDependencyForField(injectableField));
                        }

                        dependencyInfo.InjectableFields = injectableFields;

                        MethodInfo[] injectableMethods =
                            SyrupUtils.GetInjectableMethodsFromType(binding.ImplementationType);
                        foreach (MethodInfo injectableMethod in injectableMethods) {
                            foreach (ParameterInfo param in injectableMethod.GetParameters()) {
                                uniqueParameters.Add(GetNamedDependencyForParam(param));
                            }
                        }

                        dependencyInfo.InjectableMethods = injectableMethods;

                        requiredParamsCount = uniqueParameters.Count;
                    } else {
                        throw new InvalidOperationException(
                            $"Declarative binding for '{namedDependency}' is incomplete. Must call To<TImplementation>() or ToInstance().");
                    }

                    indegreesForType[namedDependency] = requiredParamsCount;
                    dependencySources[namedDependency] = dependencyInfo;
                    AddDependenciesForParam(namedDependency, uniqueParameters.ToList());
                }
            }

            //Fetch all injectable constructors
            IEnumerable<ConstructorInfo> injectedConstructors = AppDomain.CurrentDomain
                .GetAssemblies() // Returns all currently loaded assemblies
                .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
                .Where(x => x.IsClass && !x.IsAbstract) // Exclude abstract classes as well since they cannot be initiated
                .SelectMany(x => x.GetConstructors())
                .Where(x => x.GetCustomAttributes(typeof(Inject), false).FirstOrDefault() != null);

            foreach (ConstructorInfo constructor in injectedConstructors) {
                //You cannot name a constructor in constructor injection, so treat the name as null
                Type constructorDeclaringType = constructor.DeclaringType;
                NamedDependency namedDependency = new NamedDependency(null, constructor.DeclaringType);
                bool isSingleton = constructorDeclaringType.GetCustomAttribute<Singleton>() != null;

                if (indegreesForType.ContainsKey(namedDependency)) {
                    //Providers take precedence over constructors for injection.
                    //If both are provided, ignore the constructor.
                    continue;
                }

                HashSet<NamedDependency> uniqueParameters = new();
                foreach (ParameterInfo param in constructor.GetParameters()) {
                    uniqueParameters.Add(GetNamedDependencyForParam(param));
                }

                FieldInfo[] injectableFields =
                    SyrupUtils.GetInjectableFieldsFromType(constructorDeclaringType);
                foreach (FieldInfo injectableField in injectableFields) {
                    uniqueParameters.Add(GetNamedDependencyForField(injectableField));
                }

                MethodInfo[] injectableMethods =
                    SyrupUtils.GetInjectableMethodsFromType(constructorDeclaringType);
                foreach (MethodInfo injectableMethod in injectableMethods) {
                    foreach (ParameterInfo param in injectableMethod.GetParameters()) {
                        uniqueParameters.Add(GetNamedDependencyForParam(param));
                    }
                }

                indegreesForType.Add(namedDependency, uniqueParameters.Count());

                DependencyInfo dependencyInfo = new() {
                    DependencySource = DependencySource.CONSTRUCTOR,
                    Constructor = constructor,
                    Type = constructorDeclaringType,
                    IsSingleton = isSingleton,
                    InjectableMethods = injectableMethods,
                    InjectableFields = injectableFields
                };

                dependencySources[namedDependency] = dependencyInfo;
                AddDependenciesForParam(namedDependency, uniqueParameters.ToList());
            }

            ValidateDependencyGraph();
        }

        /// <summary>
        /// Goes through all params for a given dependency and marks that the dependency is a type in the graph
        /// that requires it.
        /// </summary>
        private void AddDependenciesForParam(NamedDependency namedDependency, List<NamedDependency> namedParameters) {
            foreach (NamedDependency namedParam in namedParameters) {
                HashSet<NamedDependency> dependentTypes;
                if (paramOfDependencies.ContainsKey(namedParam)) {
                    dependentTypes = paramOfDependencies[namedParam];
                } else {
                    dependentTypes = new HashSet<NamedDependency>();
                }
                dependentTypes.Add(namedDependency);
                paramOfDependencies[namedParam] = dependentTypes;
            }
        }

        /// <summary>
        /// Uses Kahn's algorithm to topologically sort the dependency graph so we can validate it
        /// from bottom to top. Objects are not actually built in this phase, only validated.
        /// I knew that my CS education would come in handy :) (jk I actually learned this one through leetcode...)
        /// </summary>
        private void ValidateDependencyGraph() {
            Queue<NamedDependency> queue = new Queue<NamedDependency>();
            Dictionary<NamedDependency, int> currentIndegrees = new Dictionary<NamedDependency, int>(indegreesForType);

            foreach (NamedDependency key in currentIndegrees.Keys.Where(key => currentIndegrees[key] == 0)) {
                queue.Enqueue(key);
            }

            int visitedCount = 0;
            while (queue.Count > 0) {
                NamedDependency namedDependency = queue.Dequeue();
                visitedCount++;

                if (!paramOfDependencies.TryGetValue(namedDependency, out HashSet<NamedDependency> dependentTypes)) {
                    continue;
                }

                foreach (NamedDependency dependentType in dependentTypes) {
                    if (!currentIndegrees.TryGetValue(dependentType, out int indegrees)) {
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
                string missingDependenciesCycle = currentIndegrees.Keys
                    .Where(namedDependency => currentIndegrees[namedDependency] > 0)
                    .Where(IsMeaningfulDependency)
                    .Aggregate("",
                        (current, namedDependency) => current +
                                                      ConstructMissingDependencyStringForType(
                                                          namedDependency));
                if (!string.IsNullOrEmpty(missingDependenciesCycle)) {
                    throw new MissingDependencyException(
                        $"Circular dependency detected or missing dependencies preventing graph completion. problematic dependencies:\n{missingDependenciesCycle}");
                }
            }

            string missingDependencies = "";
            bool incompleteGraph = false;
            foreach (NamedDependency namedDependency in indegreesForType.Keys
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
        ///     This method (and it's sister field method) should be used for
        ///     injecting fields/methods into injectable objects directly. We
        ///     want the full types for those injections.
        /// </summary>
        private NamedDependency GetNamedDependencyForParamInjection(ParameterInfo param) {
            Named dependencyName = param.GetCustomAttribute<Named>();
            string name = dependencyName != null ? dependencyName.name : null;
            Type paramType = GetContainedType(param.ParameterType);
            return new NamedDependency(name, paramType);
        }

        private NamedDependency GetNamedDependencyForFieldInjection(FieldInfo field) {
            Named dependencyName = field.GetCustomAttribute<Named>();
            string name = dependencyName != null ? dependencyName.name : null;
            Type fieldType = GetContainedType(field.FieldType);
            return new NamedDependency(name, fieldType);
        }

        /// <summary>
        /// This method (and it's sister field method) should be used for
        /// injecting fields/methods into injectable objects directly. We
        /// want the full types for those injections.
        /// </summary>
        private NamedDependency GetNamedDependencyForParam(ParameterInfo param) {
            Named dependencyName = param.GetCustomAttribute<Named>();
            string name = dependencyName?.name;
            Type paramType = GetContainedType(param.ParameterType);
            return new NamedDependency(name, paramType);
        }

        private NamedDependency GetNamedDependencyForField(FieldInfo field) {
            Named dependencyName = field.GetCustomAttribute<Named>();
            string name = dependencyName?.name;
            Type fieldType = GetContainedType(field.FieldType);
            return new NamedDependency(name, fieldType);
        }

        private Type GetContainedType(Type type) {
            if (IsLazyWrapped(type)) {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        private static bool IsLazyWrapped(Type type) {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LazyObject<>));
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

            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
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

        /// <summary>
        ///     Given a named dependency go through and iterate through the dependency's
        ///     required dependencies and build them, ultimately returning the requested
        ///     dependency.
        /// </summary>
        /// <param name="namedDependency">The NamedDependency object to build</param>
        /// <returns>The requested dependency or its LazyObject wrapped container</returns>
        private object BuildDependency(NamedDependency namedDependency) {
            NamedDependency dependencyToBuild = namedDependency;
            bool isLazy = IsLazyWrapped(namedDependency.Type);
            if (isLazy) {
                Type containedType = GetContainedType(namedDependency.Type);
                dependencyToBuild = new NamedDependency(namedDependency.Name, containedType);
                if (verboseLogging) {
                    Debug.Log($"Requested lazy instance of type: {dependencyToBuild}");
                }
            }

            if (!dependencySources.TryGetValue(dependencyToBuild, out DependencyInfo dependencyInfo)) {
                throw new MissingDependencyException(
                    $"'{dependencyToBuild}' is not a provided dependency!");
            }

            if (dependencyInfo.IsSingleton &&
                fulfilledDependencies.TryGetValue(dependencyToBuild, out object buildDependency)) {
                if (verboseLogging) {
                    Debug.Log($"Provide singleton: {dependencyToBuild}");
                }

                return buildDependency;
            }

            // Let's also check the Lazy version for this dependency.
            // Lazy containers should be singletons if the underlying type is a singleton
            // (Note: we pass in the original namedDependency param since it's already Lazy!)
            if (isLazy && dependencyInfo.IsSingleton &&
                fulfilledDependencies.TryGetValue(namedDependency, out object dependency1)) {
                if (verboseLogging) {
                    Debug.Log($"Provide lazy singleton: {namedDependency}");
                }

                return dependency1;
            }

            if (verboseLogging) {
                Debug.Log($"Constructing object: {dependencyToBuild}");
            }

            // We don't want to build the graph for lazy dependencies during the injection
            // phase, so for these dependencies we build a lazy container for them and
            // return that container directly.
            if (isLazy) {
                // Since we cannot just create arbitrary generic types Lazy<T> instances at runtime
                // we need to use reflection to create them instead.
                object lazyDependency =
                    Activator.CreateInstance(namedDependency.Type, namedDependency.Name, this);

                if (dependencyInfo.IsSingleton) {
                    fulfilledDependencies.Add(namedDependency, lazyDependency);
                }

                return lazyDependency;
            }

            object dependency;
            switch (dependencyInfo.DependencySource) {
                case DependencySource.PROVIDER: {
                    MethodInfo method = dependencyInfo.ProviderMethod;
                    object[] parameters = GetMethodParameters(method);
                    dependency = method.Invoke(dependencyInfo.ReferenceObject, parameters);
                    break;
                }
                case DependencySource.CONSTRUCTOR: {
                    ConstructorInfo constructor = dependencyInfo.Constructor;
                    object[] parameters = GetConstructorParameters(constructor);
                    dependency = constructor.Invoke(parameters);
                    InjectObject(dependency, dependencyInfo.InjectableFields,
                        dependencyInfo.InjectableMethods);
                    break;
                }
                case DependencySource.DECLARATIVE when dependencyInfo.Instance != null:
                    dependency = dependencyInfo.Instance;
                    break;
                case DependencySource.DECLARATIVE when dependencyInfo.ImplementationType != null: {
                    ConstructorInfo constructorToUse = dependencyInfo.Constructor;
                    if (constructorToUse != null) {
                        object[] parameters = GetConstructorParameters(constructorToUse);
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
                fulfilledDependencies.Add(dependencyToBuild, dependency);
            }

            return dependency;
        }

        private object[] GetMethodParameters(MethodInfo method) {
            int paramIndex = 0;
            object[] parameters = new object[method.GetParameters().Length];
            foreach (ParameterInfo parameterInfo in method.GetParameters()) {
                parameters[paramIndex] =
                    BuildDependency(GetNamedDependencyForParamInjection(parameterInfo));
                paramIndex++;
            }

            return parameters;
        }

        private object[] GetConstructorParameters(ConstructorInfo constructor) {
            int paramIndex = 0;
            object[] parameters = new object[constructor.GetParameters().Length];
            foreach (ParameterInfo parameterInfo in constructor.GetParameters()) {
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
            if (!dependencySources.TryGetValue(namedDependency, out DependencyInfo source)) {
                return false;
            }

            DependencySource dependencySource = source.DependencySource;
            return dependencySource switch {
                DependencySource.PROVIDER or DependencySource.DECLARATIVE => true,
                DependencySource.CONSTRUCTOR => paramOfDependencies.TryGetValue(namedDependency,
                    out HashSet<NamedDependency> dependency) && dependency.Any(IsMeaningfulDependency),
                _ => false
            };
        }

        private string ConstructMissingDependencyStringForType(NamedDependency namedDependency) {
            if (!dependencySources.TryGetValue(namedDependency, out DependencyInfo dependencyInfo)) {
                return $"'{namedDependency}' is not a known dependency.\n";
            }

            List<NamedDependency> parameters = new List<NamedDependency>();

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
                        foreach (MethodInfo injectableMethod in dependencyInfo.InjectableMethods) {
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
                        foreach (MethodInfo injectableMethod in dependencyInfo.InjectableMethods) {
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

            List<string> missingParams = (from namedParam in parameters
                where !dependencySources.ContainsKey(namedParam) ||
                      (indegreesForType.ContainsKey(namedParam) &&
                       indegreesForType[namedParam] > 0 && IsMeaningfulDependency(namedParam))
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
            foreach (FieldInfo injectableField in injectableFields) {
                if (verboseLogging) {
                    Debug.Log(
                        $"Injecting (Field) [{injectableField.FieldType}] into [{objectToInject.GetType()}]");
                }

                injectableField.SetValue(objectToInject,
                    BuildDependency(GetNamedDependencyForFieldInjection(injectableField)));
            }

            foreach (MethodInfo injectableMethod in injectableMethods) {
                object[] parameters = GetMethodParameters(injectableMethod);
                if (verboseLogging) {
                    string methodParams = string.Join(",",
                        parameters.Select(parameter => parameter.GetType().ToString()).ToArray());
                    Debug.Log(
                        $"Injecting (Method Params) [{methodParams}] into [{objectToInject.GetType()}]");
                }

                injectableMethod.Invoke(objectToInject, parameters);
            }
        }

        private void InjectGameObjects(List<InjectableMonoBehaviour> injectableMonoBehaviours) {
            foreach (InjectableMonoBehaviour injectableMb in injectableMonoBehaviours) {
                InjectObject(injectableMb.mb, injectableMb.fields, injectableMb.methods);
            }
        }

        private static ConstructorInfo SelectConstructorForType(Type type) {
            if (type == null || type.IsAbstract || type.IsInterface || IsStatic(type)) {
                return null;
            }

            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (!constructors.Any()) {
                return null;
            }

            List<ConstructorInfo> injectAnnotatedConstructors = constructors
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

            ConstructorInfo parameterlessConstructor =
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
    }
}
