using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeNamedSingletonModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl1>()
                .Named("SingletonService1")
                .AsSingleton();
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl2>()
                .Named("TransientService2");
        }
    }
}
