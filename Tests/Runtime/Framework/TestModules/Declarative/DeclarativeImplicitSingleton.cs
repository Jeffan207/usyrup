using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData.Declarative;

namespace Tests.Framework.TestModules {
public class DeclarativeImplicitSingleton : ISyrupModule {
    public void Configure(IBinder binder) {
        binder.Bind<SameSingleton>().AsSingleton();
        binder.Bind<ISameSingleton>().To<SameSingleton>();
    }
}
}
