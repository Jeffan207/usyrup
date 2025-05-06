using System.Collections;
using NUnit.Framework;
using Syrup.Framework;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;
using Tests.Framework.TestModules;
using UnityEngine;
using UnityEngine.TestTools;

public class LazyObjectTest {
    [Test]
    public void TestLazyObject_InjectsWithSyrupInjection_FromManuallyCreatedLazyObject() {
        var syrupInjector = new SyrupInjector(new SingleProviderModule());
        // We're passing the injector directly just for testing
        LazyObject<TastySyrup> lazyTastySyrup = new(null, syrupInjector);
        var syrup = lazyTastySyrup.Get();
        Assert.NotNull(syrup);
    }

    [Test]
    public void TestLazyObject_CachesInjectedContainedType() {
        var syrupInjector = new SyrupInjector(new SingleProviderModule());
        LazyObject<TastySyrup> lazyTastySyrup = new(null, syrupInjector);

        var syrup1 = lazyTastySyrup.Get();
        Assert.NotNull(syrup1);
        var syrup2 = lazyTastySyrup.Get();
        Assert.NotNull(syrup2);

        // The memory addresses of these two types should be equal
        // Note: the wrapped type isn't a singleton, we're just retrieving
        // the cached type from the LazyObject container.
        Assert.AreEqual(syrup1, syrup2);
    }

    [UnityTest]
    public IEnumerator TestLazyObject_UsesSyrupComponent_ToInject() {
        var sceneComponent = new GameObject();
        sceneComponent.AddComponent<FlourModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        LazyObject<Flour> lazyFlour = new();
        var flour = lazyFlour.Get();
        Assert.NotNull(flour);

        Object.Destroy(sceneComponent);
    }
}
