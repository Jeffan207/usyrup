using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData.Declarative;

namespace Tests.Framework.TestModules {
    public class DeclarativeSelfBindModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<SelfBindableConcrete>(); // Implicit self-binding
        }
    }
}
