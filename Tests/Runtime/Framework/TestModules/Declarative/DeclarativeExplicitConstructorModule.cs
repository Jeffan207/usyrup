using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeExplicitConstructorModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<DeclarativeDependency>().To<DeclarativeDependency>(); // Ensure dependency is available
            binder.Bind<ExplicitConstructorClass>().To<ExplicitConstructorClass>();
        }
    }
}
