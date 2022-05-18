using Syrup.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake() {
        ISyrupModule[] syrupModules = GetComponents<ISyrupModule>();

        if (SyrupInjector != null) {
            SyrupInjector.AddSyrupModules(syrupModules);
        } else {
            SyrupInjector = new SyrupInjector(syrupModules);
        }
    }

    private void Start() {
        if (SyrupInjector == null) {
            Debug.LogWarning("SyrupInjector has not been initialized, was it cleared between frames?");
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

    private void OnDestroy() {
        ClearInjector();
    }

    public static void ClearInjector() {
        SyrupInjector = null;
    }
}
