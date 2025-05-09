using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeSingletonModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl1>().AsSingleton();
        }
    }
}
