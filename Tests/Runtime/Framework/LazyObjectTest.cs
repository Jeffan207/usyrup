using System.Collections;
using NUnit.Framework;
using Syrup.Framework;
using Syrup.Framework.Containers;
using Syrup.Framework.Exceptions;
using Tests.Framework.TestData;
using Tests.Framework.TestModules;
using UnityEngine;
using UnityEngine.TestTools;

public class LazyObjectTest {

    [Test]
    public void TestLazyObject_InjectsWithSyrupInjection_FromManuallyCreatedLazyObject() {
        SyrupInjector syrupInjector = new SyrupInjector(new SingleProviderModule());
        // We're passing the injector directly just for testing
        LazyObject<TastySyrup> lazyTastySyrup = new(null, syrupInjector);
        TastySyrup syrup = lazyTastySyrup.Get();
        Assert.NotNull(syrup);
    }

    [Test]
    public void TestLazyObject_CachesInjectedContainedType() {
        SyrupInjector syrupInjector = new SyrupInjector(new SingleProviderModule());
        LazyObject<TastySyrup> lazyTastySyrup = new(null, syrupInjector);

        TastySyrup syrup1 = lazyTastySyrup.Get();
        Assert.NotNull(syrup1);
        TastySyrup syrup2 = lazyTastySyrup.Get();
        Assert.NotNull(syrup2);

        // The memory addresses of these two types should be equal
        // Note: the wrapped type isn't a singleton, we're just retrieving
        // the cached type from the LazyObject container.
        Assert.AreEqual(syrup1, syrup2);
    }

    [UnityTest]
    public IEnumerator TestLazyObject_UsesSyrupComponent_ToInject() {
        GameObject sceneComponent = new GameObject();
        sceneComponent.AddComponent<FlourModule>();
        sceneComponent.AddComponent<SyrupComponent>();

        yield return null;

        LazyObject<Flour> lazyFlour = new();
        Flour flour = lazyFlour.Get();
        Assert.NotNull(flour);

        UnityEngine.Object.Destroy(sceneComponent);
    }

}