using System.Collections;
using NUnit.Framework;
using Syrup.Framework;
using Syrup.Framework.Exceptions;
using Tests.Framework.TestData;
using Tests.Framework.TestModules;
using UnityEngine.TestTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class SyrupComponentTest {

    private static string EXAMPLE_SCENE_2 = "ExampleScene2";
    private static string EXAMPLE_SCENE_3 = "ExampleScene3";

    [UnitySetUp]
    public IEnumerator UnitySetUp() {
        SyrupComponent.ClearInjector();
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_AllowsMultipleComponentsInScene() {
        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<SyrupComponent>();

        GameObject secondSceneComponent = new GameObject();
        secondSceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        UnityEngine.Object.Destroy(sceneComponent);
        UnityEngine.Object.Destroy(secondSceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_AddsAdditionalModulesWhenAddingComponents() {
        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<FlourModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        Flour flour = SyrupComponent.SyrupInjector.GetInstance<Flour>();
        Assert.NotNull(flour);

        GameObject secondSceneComponent = new GameObject();
        secondSceneComponent.AddComponent<ProvidedEggModule>();
        secondSceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        //Can still get some flour
        Flour flour2 = SyrupComponent.SyrupInjector.GetInstance<Flour>();
        Assert.NotNull(flour2);

        //And also eggs now!
        Egg egg = SyrupComponent.SyrupInjector.GetInstance<Egg>();
        Assert.NotNull(egg);

        UnityEngine.Object.Destroy(sceneComponent);
        UnityEngine.Object.Destroy(secondSceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_OnlyInjectsCurrentScenes() {
        //The initialize scene injection
        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<ProvidedEggModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        Egg egg = SyrupComponent.SyrupInjector.GetInstance<Egg>();
        Assert.NotNull(egg);

        Assert.Throws<MissingDependencyException>(() => {
            //No flour just yet!
            SyrupComponent.SyrupInjector.GetInstance<Flour>();
        });

        yield return null;

        //The SyrupComponent in ExampleScene2 should be setup so it only injects itself
        SceneManager.LoadScene(EXAMPLE_SCENE_2, LoadSceneMode.Additive);

        yield return null;

        Egg egg2 = SyrupComponent.SyrupInjector.GetInstance<Egg>();
        Assert.NotNull(egg2);

        //Can get some flour now too
        Flour flour = SyrupComponent.SyrupInjector.GetInstance<Flour>();
        Assert.NotNull(flour);

        //Clean up the scene and the created game objects
        yield return SceneManager.UnloadSceneAsync(EXAMPLE_SCENE_2);
        UnityEngine.Object.Destroy(sceneComponent);        
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_CanInjectGameObjectsInTheScene() {
        GameObject goodPancake1 = new GameObject();
        goodPancake1.AddComponent<GoodPancake>();

        GameObject goodPancake2 = new GameObject();
        goodPancake2.AddComponent<GoodPancake>();

        GameObject badPancake1 = new GameObject();
        badPancake1.AddComponent<BadPancake>();

        GameObject badPancake2 = new GameObject();
        badPancake2.AddComponent<BadPancake>();

        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<ExampleSyrupModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        PureMapleSyrup pureMapleSyrup1 = goodPancake1.GetComponent<GoodPancake>().pureMapleSyrup;
        PureMapleSyrup pureMapleSyrup2 = goodPancake2.GetComponent<GoodPancake>().pureMapleSyrup;

        Assert.NotNull(pureMapleSyrup1);
        Assert.NotNull(pureMapleSyrup2);
        Assert.AreNotEqual(pureMapleSyrup1.id, pureMapleSyrup2.id);
        Assert.AreNotEqual(pureMapleSyrup1.mapleSap.id, pureMapleSyrup2.mapleSap.id);

        HighFructoseCornSyrup badSyrup1 = badPancake1.GetComponent<BadPancake>().highFructoseCornSyrup;
        HighFructoseCornSyrup badSyrup2 = badPancake2.GetComponent<BadPancake>().highFructoseCornSyrup;

        Assert.NotNull(badSyrup1);
        Assert.NotNull(badSyrup2);        
        Assert.AreEqual(badSyrup1.id, badSyrup2.id);

        UnityEngine.Object.Destroy(goodPancake1);
        UnityEngine.Object.Destroy(goodPancake2);
        UnityEngine.Object.Destroy(badPancake1);
        UnityEngine.Object.Destroy(badPancake2);
        UnityEngine.Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_ProvidesAnInjectorThatCanBeUsed() {
        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<ExampleSyrupModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        HighFructoseCornSyrup syrup = SyrupComponent.SyrupInjector.GetInstance<HighFructoseCornSyrup>();

        Assert.NotNull(syrup);

        yield return null;

        UnityEngine.Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_InjectsWhenSceneInjectionIsExplicitlyEnabled() {
        GameObject toast = new GameObject();
        toast.AddComponent<Toast>();

        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        Butter butter = toast.GetComponent<Toast>().butter;

        Assert.NotNull(butter);

        UnityEngine.Object.Destroy(toast);
        UnityEngine.Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_DoesNotInjectWhenSceneInjectionIsDisabledOnTheInjectableObject() {
        GameObject bagel = new GameObject();
        bagel.AddComponent<Bagel>();

        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        Butter butter = bagel.GetComponent<Bagel>().butter;

        Assert.Null(butter);

        UnityEngine.Object.Destroy(bagel);
        UnityEngine.Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_DoesNotInjectWhenSceneInjectionIsDisabledOnTheComponent() {
        GameObject toast = new GameObject();
        toast.AddComponent<Toast>(); //SceneInjection is explicitly enabled on Toast

        //The SyrupComponent in ExampleScene3 is setup to have scene injection disabled
        SceneManager.LoadScene(EXAMPLE_SCENE_3, LoadSceneMode.Additive);

        yield return null;

        Butter butter = toast.GetComponent<Toast>().butter;

        Assert.Null(butter);

        UnityEngine.Object.Destroy(toast);        
    }

    /// <summary>
    /// This test probably doesn't belong here.
    /// It tests that a MonoBehaviour's Start() can call SyrupComponent.SyrupInjector.Inject(this).
    /// </summary>
    [UnityTest]
    public IEnumerator TestSyrupComponent_WithOnDemandInjection() {
        //Create the SyrupComponent first and wait a frame that way it will go through its injection loop
        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<SyrupComponent>();
        yield return null;

        //The AutoToast will be injected via on-demand injection
        GameObject autoToast = new GameObject();
        autoToast.AddComponent<AutoToast>();

        //Wait a frame so it goes through its Start() method.
        yield return null;

        AutoToast autoToastComponent = autoToast.GetComponent<AutoToast>();

        Assert.NotNull(autoToastComponent.butter);

        UnityEngine.Object.Destroy(autoToast);
        UnityEngine.Object.Destroy(sceneComponent);
    }

}
