using Syrup.Framework;
using Syrup.Framework.Attributes;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeInjectSpecificCtorHybridModule : ISyrupModule {
        [Provides]
        public IDeclarativeService ProvideServiceWithDependency(DeclarativeDependency dep) =>
            new DeclarativeServiceImpl1(dep);

        public void Configure(IBinder binder) {
            binder.Bind<DeclarativeDependency>().To<DeclarativeDependency>();
        }
    }
}
