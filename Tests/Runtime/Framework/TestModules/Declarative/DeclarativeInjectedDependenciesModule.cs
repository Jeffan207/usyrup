using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeInjectedDependenciesModule : ISyrupModule {
        public void Configure(IBinder binder) {
            // DeclarativeServiceImpl1 has a constructor that takes DeclarativeDependency
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl1>();
            // DeclarativeDependency will be constructor injected (assuming it has a suitable public constructor)
            // or it can be explicitly bound if needed:
            binder.Bind<DeclarativeDependency>().To<DeclarativeDependency>();
        }
    }
}
