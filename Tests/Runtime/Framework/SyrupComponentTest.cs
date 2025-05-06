using System.Collections;
using NUnit.Framework;
using Syrup.Framework;
using Syrup.Framework.Containers;
using Syrup.Framework.Exceptions;
using Tests.Framework.TestData;
using Tests.Framework.TestModules;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SyrupComponentTest {
    private static readonly string EXAMPLE_SCENE_2 = "ExampleScene2";
    private static readonly string EXAMPLE_SCENE_3 = "ExampleScene3";

    [UnitySetUp]
    public IEnumerator UnitySetUp() {
        SyrupComponent.ClearInjector();
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_AllowsMultipleComponentsInScene() {
        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<SyrupComponent>();

        var secondSceneComponent = new GameObject();
        secondSceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        Object.Destroy(sceneComponent);
        Object.Destroy(secondSceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_AddsAdditionalModulesWhenAddingComponents() {
        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<FlourModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        var flour = SyrupComponent.SyrupInjector.GetInstance<Flour>();
        Assert.NotNull(flour);

        var secondSceneComponent = new GameObject();
        secondSceneComponent.AddComponent<ProvidedEggModule>();
        secondSceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        //Can still get some flour
        var flour2 = SyrupComponent.SyrupInjector.GetInstance<Flour>();
        Assert.NotNull(flour2);

        //And also eggs now!
        var egg = SyrupComponent.SyrupInjector.GetInstance<Egg>();
        Assert.NotNull(egg);

        Object.Destroy(sceneComponent);
        Object.Destroy(secondSceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_OnlyInjectsCurrentScenes() {
        //The initialize scene injection
        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<ProvidedEggModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        var egg = SyrupComponent.SyrupInjector.GetInstance<Egg>();
        Assert.NotNull(egg);

        Assert.Throws<MissingDependencyException>(() => {
            //No flour just yet!
            SyrupComponent.SyrupInjector.GetInstance<Flour>();
        });

        yield return null;

        //The SyrupComponent in ExampleScene2 should be setup so it only injects itself
        SceneManager.LoadScene(EXAMPLE_SCENE_2, LoadSceneMode.Additive);

        yield return null;

        var egg2 = SyrupComponent.SyrupInjector.GetInstance<Egg>();
        Assert.NotNull(egg2);

        //Can get some flour now too
        var flour = SyrupComponent.SyrupInjector.GetInstance<Flour>();
        Assert.NotNull(flour);

        //Clean up the scene and the created game objects
        yield return SceneManager.UnloadSceneAsync(EXAMPLE_SCENE_2);
        Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_CanInjectGameObjectsInTheScene() {
        var goodPancake1 = new GameObject();
        goodPancake1.AddComponent<GoodPancake>();

        var goodPancake2 = new GameObject();
        goodPancake2.AddComponent<GoodPancake>();

        var badPancake1 = new GameObject();
        badPancake1.AddComponent<BadPancake>();

        var badPancake2 = new GameObject();
        badPancake2.AddComponent<BadPancake>();

        var sceneComponent = new GameObject();
        // The flour was a later test addition so it gets its own module
        sceneComponent.AddComponent<FlourModule>();
        sceneComponent.AddComponent<ExampleSyrupModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        var pureMapleSyrup1 = goodPancake1.GetComponent<GoodPancake>().pureMapleSyrup;
        var pureMapleSyrup2 = goodPancake2.GetComponent<GoodPancake>().pureMapleSyrup;

        Assert.NotNull(pureMapleSyrup1);
        Assert.NotNull(pureMapleSyrup2);
        Assert.AreNotEqual(pureMapleSyrup1.id, pureMapleSyrup2.id);
        Assert.AreNotEqual(pureMapleSyrup1.mapleSap.id, pureMapleSyrup2.mapleSap.id);

        var badSyrup1 = badPancake1.GetComponent<BadPancake>().highFructoseCornSyrup;
        var badSyrup2 = badPancake2.GetComponent<BadPancake>().highFructoseCornSyrup;

        Assert.NotNull(badSyrup1);
        Assert.NotNull(badSyrup2);
        Assert.AreEqual(badSyrup1.id, badSyrup2.id);

        var lazyFlour1 = badPancake1.GetComponent<BadPancake>().lazyFlour;
        var lazyFlour2 = badPancake2.GetComponent<BadPancake>().lazyFlour;

        Assert.NotNull(lazyFlour1);
        Assert.NotNull(lazyFlour2);
        var flour1 = lazyFlour1.Get();
        var flour2 = lazyFlour2.Get();
        Assert.AreNotEqual(flour1.id, flour2.id);

        Object.Destroy(goodPancake1);
        Object.Destroy(goodPancake2);
        Object.Destroy(badPancake1);
        Object.Destroy(badPancake2);
        Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_ProvidesAnInjectorThatCanBeUsed() {
        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<ExampleSyrupModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        var syrup = SyrupComponent.SyrupInjector.GetInstance<HighFructoseCornSyrup>();

        Assert.NotNull(syrup);

        yield return null;

        Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_InjectsWhenSceneInjectionIsExplicitlyEnabled() {
        var toast = new GameObject();
        toast.AddComponent<Toast>();

        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        var butter = toast.GetComponent<Toast>().butter;

        Assert.NotNull(butter);

        Object.Destroy(toast);
        Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator
        TestSyrupComponent_DoesNotInjectWhenSceneInjectionIsDisabledOnTheInjectableObject() {
        var bagel = new GameObject();
        bagel.AddComponent<Bagel>();

        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        var butter = bagel.GetComponent<Bagel>().butter;

        Assert.Null(butter);

        Object.Destroy(bagel);
        Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator
        TestSyrupComponent_DoesNotInjectWhenSceneInjectionIsDisabledOnTheComponent() {
        var toast = new GameObject();
        toast.AddComponent<Toast>(); //SceneInjection is explicitly enabled on Toast

        //The SyrupComponent in ExampleScene3 is setup to have scene injection disabled
        SceneManager.LoadScene(EXAMPLE_SCENE_3, LoadSceneMode.Additive);

        yield return null;

        var butter = toast.GetComponent<Toast>().butter;

        Assert.Null(butter);

        Object.Destroy(toast);
    }

    /// <summary>
    ///     This test probably doesn't belong here.
    ///     It tests that a MonoBehaviour's Start() can call SyrupComponent.SyrupInjector.Inject(this).
    /// </summary>
    [UnityTest]
    public IEnumerator TestSyrupComponent_WithOnDemandInjection() {
        //Create the SyrupComponent first and wait a frame that way it will go through its injection loop
        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<SyrupComponent>();
        yield return null;

        //The AutoToast will be injected via on-demand injection
        var autoToast = new GameObject();
        autoToast.AddComponent<AutoToast>();

        //Wait a frame so it goes through its Start() method.
        yield return null;

        var autoToastComponent = autoToast.GetComponent<AutoToast>();

        Assert.NotNull(autoToastComponent.butter);

        Object.Destroy(autoToast);
        Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_CanBeFetchedWithinLazyObject() {
        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<FlourModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        LazyObject<Flour> lazyFlour = new();
        var flour = lazyFlour.Get();
        Assert.NotNull(flour);

        Object.Destroy(sceneComponent);
    }

    [UnityTest]
    public IEnumerator TestSyrupComponent_AwakeInjection() {
        var goodPancake = new GameObject();
        goodPancake.AddComponent<GoodPancake>();

        var sceneComponent = new GameObject();

        // Deactivate so awake doesn't immiedately run injection
        sceneComponent.SetActive(false);
        sceneComponent.AddComponent<ExampleSyrupModule>();
        sceneComponent.AddComponent<SyrupComponent>();
        sceneComponent.GetComponent<SyrupComponent>().SetInjectInAwake(true);
        sceneComponent.SetActive(true);

        // Injection should be done in the Awake() method after enabled
        yield return null;

        var pureMapleSyrup = goodPancake.GetComponent<GoodPancake>().pureMapleSyrup;
        Assert.NotNull(pureMapleSyrup);

        Object.Destroy(goodPancake);
        Object.Destroy(sceneComponent);
    }
}
