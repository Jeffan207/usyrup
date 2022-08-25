using System;
using System.Linq;
using Syrup.Framework.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Syrup.Framework.Attributes;
using System.Reflection;

namespace Syrup.Framework {

    /// <summary>
    /// Contains utilities used by USyrup.
    /// </summary>
    internal class SyrupUtils {

        /// <summary>
        /// Returns a list of MonoBehaviours in the provided scene that have an injectable
        /// entry point (currently only method injection). All MonoBehaviours are then added to the inputted list.
        /// </summary>
        /// <param name="injectableMonoBehaviours">List to be populated with injectable MonoBehaviours</param>
        internal static void GetInjectableMonoBehaviours(Scene scene, List<InjectableMonoBehaviour> injectableMonoBehaviours) {

            //Unlike Zenject we inject our dependencies in Start so we can call GetRootGameObjects without worry
            //You may be thinking we should initialize our GameObjects in Awake and you're probably right, but this is easier.
            foreach (var rootObj in scene.GetRootGameObjects()) {
                if (rootObj != null) {
                    GetInjectableMonoBehavioursUnderObject(rootObj, injectableMonoBehaviours);
                }
            }
        }

        private static void GetInjectableMonoBehavioursUnderObject(
            GameObject gameObject, List<InjectableMonoBehaviour> injectableMonoBehaviours) {
            if (gameObject == null) {
                return;
            }

            var monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

            for (int i = 0; i < monoBehaviours.Length; i++) {
                var monoBehaviour = monoBehaviours[i];

                //MonoBehaviours under our SyrupModules are not injectable as they're assumed to be fully formed.
                //I.E. they're supposed to be MonoBehaviours that are returned by a ProviderMethod.
                //TODO: enable implicit providing of MonoBehaviours under our module (and then figure out when to inject
                //them before fulfilling our other dependencies)
                if (monoBehaviour != null && monoBehaviour.GetType().IsAssignableFrom(typeof(ISyrupModule))) {
                    return;
                }
            }

            // Recurse first so it adds components bottom up though it shouldn't really matter much
            // because it should always inject in the dependency order
            for (int i = 0; i < gameObject.transform.childCount; i++) {
                var child = gameObject.transform.GetChild(i);

                if (child != null) {
                    GetInjectableMonoBehavioursUnderObject(child.gameObject, injectableMonoBehaviours);
                }
            }

            for (int i = 0; i < monoBehaviours.Length; i++) {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null && monoBehaviour.GetType() != null) {
                    Type monoBehaviourType = monoBehaviour.GetType();
                    if (monoBehaviourType.GetCustomAttribute<SceneInjection>() != null &&
                        !monoBehaviourType.GetCustomAttribute<SceneInjection>().enabled) {
                        //This MonoBehaviour disabled scene injection, so skip injecting it.
                        continue;
                    }

                    var injectableMethods = GetInjectableMethodsFromType(monoBehaviourType);
                    if (injectableMethods.Length > 0) {
                        injectableMonoBehaviours.Add(new InjectableMonoBehaviour(monoBehaviour, injectableMethods));
                    }
                }
            }

        }     

        public static MethodInfo[] GetInjectableMethodsFromType(Type t) {
            var injectableMethods = t.GetMethods()
                            .Where(x => x.GetCustomAttributes(typeof(Inject), false).FirstOrDefault() != null);
            return injectableMethods.Reverse().ToArray();
        }    
    }
}