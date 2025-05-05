using UnityEngine;
using UnityEngine.SceneManagement;

namespace Syrup.Framework {
    public class SyrupComponent : MonoBehaviour {

        /// <summary>
        /// A singleton SyrupInjector thats managed by the SyrupComponent class. If multiple SyrupComponents are loaded
        /// into the same scene they will use the same SyrupInjector to load their modules into. If the scene is ended
        /// the SyrupInjector is destroyed.
        /// </summary>
        /// <value></value>
        public static SyrupInjector SyrupInjector { get; private set; }

        [SerializeField]
        [Tooltip("List the scenes that this component is specifically designated to inject. " +
        "If none are set, the component will attempt to inject all loaded scenes. " +
        "You should explicitly set this if you want to dynamically load scenes that have their " +
        "own SyrupComponents as it will prevent the components from reinjecting already injected objects!")]
        private string[] scenesToInject;

        [SerializeField]
        [Tooltip("When enabled this componenet will inject scenes when the componenet is loaded. If this is disabled " +
            "then objects in the scene will need to rely on the SyrupInjector directly to fulfill their dependencies.")]
        private bool useSceneInjection = true;

        [SerializeField]
        [Tooltip("Enable/disables verbose console logging for both the SyrupComponent and SyrupInjector.")]
        private bool verboseLogging = false;

        [SerializeField]
        [Tooltip("Allows Syrup Component to start injecting in Awake method instead of the Start method. " +
                 "This way you will be able to use the injected objects in methods like OnEnable.")]
        private bool injectInAwake = false;

        private void Awake() {
            ISyrupModule[] syrupModules = GetComponents<ISyrupModule>();

            if (SyrupInjector != null) {
                SyrupInjector.AddSyrupModules(syrupModules);
            } else {
                SyrupInjectorOptions syrupInjectorOptions = new();
                syrupInjectorOptions.VerboseLogging = verboseLogging;
                SyrupInjector = new SyrupInjector(syrupInjectorOptions, syrupModules);
            }

            if (injectInAwake) {
                StartInject();
            }
        }

        private void Start() {
            if (!injectInAwake) {
                StartInject();
            }
        }

        private void OnDestroy() {
            ClearInjector();
        }

        public static void ClearInjector() {
            SyrupInjector = null;
        }

        private void StartInject() {
            if (SyrupInjector == null) {
                Debug.LogWarning("SyrupInjector has not been initialized, was it cleared between frames?");
                return;
            }

            if (!useSceneInjection) {
                if (verboseLogging) {
                    Debug.Log("Scene injection has been disabled, skipping injection...");
                }
                return;
            }

            if (scenesToInject != null && scenesToInject.Length > 0) {
                foreach (string scene in scenesToInject) {
                    SyrupInjector.InjectGameObjectsInScene(SceneManager.GetSceneByName(scene));
                }
            } else {
                SyrupInjector.InjectAllGameObjects();
            }
        }

        public void SetInjectInAwake(bool enabled) {
            injectInAwake = enabled;
        }
    }
}
