using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeSelfBindSingletonModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<DeclarativeDependency>().To<DeclarativeDependency>().AsSingleton();
        }
    }
}
