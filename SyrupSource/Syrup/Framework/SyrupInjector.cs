using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Syrup.Framework.Exceptions;
using Syrup.Framework.Attributes;
using Syrup.Framework.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using Syrup.Framework.Containers;

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

        internal void AddSyrupModules(params ISyrupModule[] modules) {
            indegreesForType = new Dictionary<NamedDependency, int>();

            //Fetch all provider methods declared in all provided modules
            foreach (ISyrupModule module in modules) {
                var providerMethods = module.GetType().GetMethods()
                    .Where(x => x.GetCustomAttributes(typeof(Provides), false).FirstOrDefault() != null);

                foreach (MethodInfo methodInfo in providerMethods) {
                    if (IsLazyWrapped(methodInfo.ReturnType)) {
                        throw new InvalidProvidedDependencyException(string.Format(
                                "A provider is trying to provide a Lazy type explicitly for '{0}', try pushing the Lazy type down onto the consuming class instead",
                                methodInfo.ReturnType.FullName));
                    }

                    Named dependencyName = methodInfo.GetCustomAttribute<Named>();
                    string name = dependencyName != null ? dependencyName.name : null;
                    NamedDependency namedDependency = new NamedDependency(name, methodInfo.ReturnType);
                    bool isSingleton = methodInfo.GetCustomAttribute<Singleton>() != null;

                    //This is a convenient place to check if multiple providers for a single type have been declared
                    if (indegreesForType.ContainsKey(namedDependency)) {
                        throw new DuplicateProviderException
                            (string.Format("A provider for the specified dependency '{0}' already exists!",
                            namedDependency));
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

            //Fetch all injectable constructors
            var injectedConstructors = AppDomain.CurrentDomain
                .GetAssemblies() // Returns all currenlty loaded assemblies
                .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetConstructors())
                .Where(x => x.GetCustomAttributes(typeof(Inject), false).FirstOrDefault() != null);

            foreach (ConstructorInfo constructor in injectedConstructors) {

                //You cannot name a constructor in constructor injection, so treat the name as null
                NamedDependency namedDependency = new NamedDependency(null, constructor.DeclaringType);
                bool isSingleton = constructor.DeclaringType.GetCustomAttribute<Singleton>() != null;

                if (indegreesForType.ContainsKey(namedDependency)) {
                    //Providers take precedence over constructors for injection.
                    //If both are provided, ignore the constructor.
                    continue;
                }

                HashSet<NamedDependency> uniqueParameters = new();
                foreach (ParameterInfo param in constructor.GetParameters()) {
                    uniqueParameters.Add(GetNamedDependencyForParam(param));
                }

                var injectableFields = SyrupUtils.GetInjectableFieldsFromType(constructor.DeclaringType);
                foreach (FieldInfo injectableField in injectableFields) {
                    uniqueParameters.Add(GetNamedDependencyForField(injectableField));
                }

                var injectableMethods = SyrupUtils.GetInjectableMethodsFromType(constructor.DeclaringType);               
                foreach (MethodInfo injectableMethod in injectableMethods) {
                    foreach (ParameterInfo param in injectableMethod.GetParameters()) {
                        uniqueParameters.Add(GetNamedDependencyForParam(param));
                    }
                }

                indegreesForType.Add(namedDependency, uniqueParameters.Count());

                DependencyInfo dependencyInfo = new() {
                    DependencySource = DependencySource.CONSTRUCTOR,
                    Constructor = constructor,
                    Type = constructor.DeclaringType,
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
            foreach (NamedDependency key in indegreesForType.Keys) {
                if (indegreesForType[key] == 0) {
                    queue.Enqueue(key);
                }
            }

            while (queue.Count > 0) {
                NamedDependency namedDependency = queue.Dequeue();
                if (!paramOfDependencies.ContainsKey(namedDependency)) {
                    continue;
                }

                HashSet<NamedDependency> dependentTypes = paramOfDependencies[namedDependency];
                foreach (NamedDependency dependentType in dependentTypes) {
                    int indegrees = indegreesForType[dependentType];
                    indegrees--;

                    if (indegrees == 0) {
                        queue.Enqueue(dependentType);
                    }

                    indegreesForType[dependentType] = indegrees;
                }
            }

            string missingDependencies = "";
            bool incompleteGraph = false;
            foreach (NamedDependency namedDependency in indegreesForType.Keys) {
                if (indegreesForType[namedDependency] > 0) {

                    if (!IsMeaningfulDependency(namedDependency)) {
                        //Even though we don't fully provide this dependency it's not explicitly declared by
                        //one of the passed modules in the graph, so we can ignore it
                        continue;
                    }

                    missingDependencies += ConstructMissingDependencyStringForType(namedDependency);
                    incompleteGraph = true;
                }
            }

            if (incompleteGraph) {
                throw new MissingDependencyException(
                    string.Format(
                        "Incomplete dependency graph. The following dependencies are missing from the completed graph:\n{0}",
                    missingDependencies));
            }
        }        

        /// <summary>
        /// This method (and it's sister field method) should be used for
        /// building the dependency heirarchy. For that process we want to
        /// discard any containers so we can build the underlying types.
        /// </summary>
        private NamedDependency GetNamedDependencyForParam(ParameterInfo param) {
            Named dependencyName = param.GetCustomAttribute<Named>();
            string name = dependencyName != null ? dependencyName.name : null;           
            Type paramType = GetContainedType(param.ParameterType);
            return new NamedDependency(name, paramType);
        }

        private NamedDependency GetNamedDependencyForField(FieldInfo field) {
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
        private NamedDependency GetNamedDependencyForParamInjection(ParameterInfo param) {
            Named dependencyName = param.GetCustomAttribute<Named>();
            string name = dependencyName != null ? dependencyName.name : null;
            return new NamedDependency(name, param.ParameterType);
        }

        private NamedDependency GetNamedDependencyForFieldInjection(FieldInfo field) {
            Named dependencyName = field.GetCustomAttribute<Named>();
            string name = dependencyName != null ? dependencyName.name : null;
            return new NamedDependency(name, field.FieldType);
        }

        private Type GetContainedType(Type type) {
            if (IsLazyWrapped(type)) {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        private bool IsLazyWrapped(Type type) {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LazyObject<>));
        }

        /// <summary>
        /// Given a named dependency go through and iterate through the dependency's
        /// required dependencies and build them, ultimately returning the requested
        /// dependency.
        /// </summary>
        /// <param name="namedDependency">The NamedDependency object to build</param>
        /// <returns>The requested dependency or its LazyObject wrapped container</returns>
        private object BuildDependency(NamedDependency namedDependency) {


            NamedDependency dependencyToBuild = namedDependency;
            bool isLazy = IsLazyWrapped(namedDependency.type);
            if (isLazy) {                
                Type containedType = GetContainedType(namedDependency.type);                
                dependencyToBuild = new NamedDependency(namedDependency.name, containedType);
                if (verboseLogging) {
                    Debug.Log(string.Format("Requested lazy instance of type: {0}", dependencyToBuild));
                }
            }
            

            if (!dependencySources.ContainsKey(dependencyToBuild)) {
                throw new MissingDependencyException(string.Format("'{0}' is not a provided dependency!", dependencyToBuild));
            }

            DependencyInfo dependencyInfo = dependencySources[dependencyToBuild];
            if (dependencyInfo.IsSingleton && fulfilledDependencies.ContainsKey(dependencyToBuild)) {
                if (verboseLogging) {
                    Debug.Log(string.Format("Provide singleton: {0}", dependencyToBuild.ToString()));
                }
                return fulfilledDependencies[dependencyToBuild];
            }
            // Let's also check the Lazy version for this dependency.
            // Lazy containers should be singletons if the underlying type is a singleton
            // (Note: we pass in the original namedDependency param since it's already Lazy!)
            if (isLazy && dependencyInfo.IsSingleton && fulfilledDependencies.ContainsKey(namedDependency)) {
                if (verboseLogging) {
                    Debug.Log(string.Format("Provide lazy singleton: {0}", namedDependency.ToString()));
                }
                return fulfilledDependencies[namedDependency];
            }

            if (verboseLogging) {
                Debug.Log(string.Format("Constructing object: {0}", dependencyToBuild.ToString()));
            }

            // We don't want to build the graph for lazy dependencies during the injection
            // phase, so for these dependencies we build a lazy container for them and
            // return that container directly.
            if (isLazy) {
                // Since we cannot just create arbitrary generic types Lazy<T> instances at runtime
                // we need to use reflection to create them instead.
                object lazyDependency = Activator.CreateInstance(namedDependency.type, namedDependency.name, this);

                if (dependencyInfo.IsSingleton) {
                    fulfilledDependencies.Add(namedDependency, lazyDependency);
                }

                return lazyDependency;
            }

            object dependency;
            if (dependencyInfo.DependencySource == DependencySource.PROVIDER) {
                MethodInfo method = dependencyInfo.ProviderMethod;
                object[] parameters = GetMethodParameters(method);
                dependency = method.Invoke(dependencyInfo.ReferenceObject, parameters);
            } else if (dependencyInfo.DependencySource == DependencySource.CONSTRUCTOR) {
                ConstructorInfo constructor = dependencyInfo.Constructor;
                object[] parameters = GetConstructorParameters(constructor);
                dependency = constructor.Invoke(parameters);
                InjectObject(dependency, dependencyInfo.InjectableFields, dependencyInfo.InjectableMethods);
            } else {
                throw new UnknownDependencySourceException(
                    string.Format("Unknown DependencySource: '{0}', cannot fulfill dependency!", dependencyInfo.DependencySource));
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
                parameters[paramIndex] = BuildDependency(GetNamedDependencyForParamInjection(parameterInfo));
                paramIndex++;
            }
            return parameters;
        }

        private object[] GetConstructorParameters(ConstructorInfo constructor) {
            int paramIndex = 0;
            object[] parameters = new object[constructor.GetParameters().Length];
            foreach (ParameterInfo parameterInfo in constructor.GetParameters()) {
                parameters[paramIndex] = BuildDependency(GetNamedDependencyForParamInjection(parameterInfo));
                paramIndex++;
            }
            return parameters;
        }

        /// <summary>
        /// A meaningful dependency is one that the module needs to provide because it directly rolls up
        /// into a Provider annotated method in the module itself. Some constructor injected objects might not
        /// be provided by the module, so we need to validate if they should fail graph validation or not
        /// </summary>
        private bool IsMeaningfulDependency(NamedDependency namedDependency) {
            if (dependencySources[namedDependency].DependencySource == DependencySource.PROVIDER) {
                //Providers are always meaningful in the context of a module
                return true;
            } else { //must be constructor injection or other
                if (!paramOfDependencies.ContainsKey(namedDependency)) {
                    //Nothing depends on this dependency, so its not meaningful
                    return false;
                }

                //If this dependencies has other dependencies that need it then check if those are meaningful
                //If one of them is, then this is also a meaningful dependency.
                foreach (NamedDependency param in paramOfDependencies[namedDependency]) {
                    if (IsMeaningfulDependency(param)) {
                        return true;
                    }
                }
                return false;
            }
        }

        private string ConstructMissingDependencyStringForType(NamedDependency namedDependency) {
            DependencyInfo dependencyInfo = dependencySources[namedDependency];
            List<NamedDependency> parameters;
            if (dependencyInfo.DependencySource == DependencySource.PROVIDER) {
                parameters = dependencyInfo.ProviderMethod.GetParameters()
                    .Select(param => GetNamedDependencyForParam(param))
                    .ToList();
            } else if (dependencyInfo.DependencySource == DependencySource.CONSTRUCTOR) {
                parameters = new();
                parameters.AddRange(
                    dependencyInfo.Constructor.GetParameters()
                        .Select(param => GetNamedDependencyForParam(param))
                        .ToList());
                foreach (MethodInfo injectableMethod in dependencyInfo.InjectableMethods) {
                    parameters.AddRange(
                        injectableMethod.GetParameters()
                            .Select(param => GetNamedDependencyForParam(param))
                            .ToList());
                }
            } else {
                return string.Format("'{0}' invalid dependency source\n", namedDependency);
            }

            List<string> missingParams = new();
            foreach (NamedDependency namedParam in parameters) {
                if (!dependencySources.ContainsKey(namedParam) ||
                        (indegreesForType.ContainsKey(namedParam) && indegreesForType[namedParam] > 0)) {
                    missingParams.Add(namedParam.ToString());
                }
            }

            return string.Format("'{0}' is missing the following dependencies: '{1}'\n",
                namedDependency,
                string.Join(',', missingParams));
        }

        /// <summary>
        /// Retrieves an instance of object of type T from the dependency graph.
        /// Objects will be built as needed.
        /// </summary>
        /// <returns>The object of type T build from the dependency graph</returns>
        public T GetInstance<T>() {
            return GetInstance<T>(null);
        }

        /// <summary>
        /// Returns a named dependency from the dependency graph. Passing null is equivalent to requesting an
        /// unnamed dependency of that type.
        /// </summary>
        /// <typeparam name="T">The name of the type if explicitly given one with the [Named] attribute</typeparam>
        /// <returns></returns>
        public T GetInstance<T>(string name) {
            NamedDependency namedDependency = new NamedDependency(name, typeof(T));
            object dependency = BuildDependency(namedDependency);
            return (T)dependency;
        }

        /// <summary>
        /// Injects dependencies into the provided object by invoking any inject related methods.
        /// </summary>
        /// <param name="objectToInject">The object to be injected</param>
        public void Inject<T>(T objectToInject) {
            FieldInfo[] injectableFields = SyrupUtils.GetInjectableFieldsFromType(objectToInject.GetType());
            MethodInfo[] injectableMethods = SyrupUtils.GetInjectableMethodsFromType(objectToInject.GetType());
            InjectObject(objectToInject, injectableFields, injectableMethods);
        }

        private void InjectObject<T>(T objectToInject, FieldInfo[] injectableFields, MethodInfo[] injectableMethods) {
            //We're making the assumption that the injectable fields/methods are ordered from base class
            //to deriving class (they should be) but we're assuming it too.
            foreach (FieldInfo injectableField in injectableFields) {
                if (verboseLogging) {
                    Debug.Log(string.Format("Injecting (Field) [{0}] into [{1}]", injectableField.FieldType, objectToInject.GetType()));
                }
                injectableField.SetValue(objectToInject, BuildDependency(GetNamedDependencyForFieldInjection(injectableField)));
            }

            foreach (MethodInfo injectableMethod in injectableMethods) {
                object[] parameters = GetMethodParameters(injectableMethod);
                if (verboseLogging) {
                    string methodParams = string.Join(",", parameters.Select(parameter => parameter.GetType().ToString()).ToArray());
                    Debug.Log(string.Format("Injecting (Method Params) [{0}] into [{1}]", methodParams, objectToInject.GetType()));
                }
                injectableMethod.Invoke(objectToInject, parameters);
            }
        }


        // BEGIN ALL UNITY SPECIFIC INJECTION PATTERNS

        /// <summary>
        /// Injects all methods attached to MBs within all scenes that have methods annotated with the [Inject]
        /// attribute. In order to prevent duplicate injections this should only be called from within the
        /// SyrupComponent. Method injection ignores the return type of the injected method, so don't rely on it
        /// to fulfill dependencies for the dependency graph inside the injector.
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
        /// Injects all methods in a specified scene that have methods annotated with the [Inject] attribute.
        /// Use this if you plan to dynamically add scenes that will need to be injected outside the games
        /// initial setup steps.
        /// </summary>
        internal void InjectGameObjectsInScene(Scene scene) {
            List<InjectableMonoBehaviour> injectableMonoBehaviours = new();
            SyrupUtils.GetInjectableMonoBehaviours(scene, injectableMonoBehaviours);
            InjectGameObjects(injectableMonoBehaviours);
        }

        private void InjectGameObjects(List<InjectableMonoBehaviour> injectableMonoBehaviours) {
            foreach (InjectableMonoBehaviour injectableMb in injectableMonoBehaviours) {
                InjectObject(injectableMb.mb, injectableMb.fields, injectableMb.methods);
            }
        }
    }
}
