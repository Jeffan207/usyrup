using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeNamedModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl1>().Named("Service1");
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl2>().Named("Service2");
        }
    }
}
