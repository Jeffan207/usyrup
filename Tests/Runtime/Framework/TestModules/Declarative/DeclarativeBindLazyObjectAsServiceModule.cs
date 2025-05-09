using Syrup.Framework;
using Syrup.Framework.Containers;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData.Declarative;

namespace Tests.Framework.TestModules {
    public class DeclarativeBindLazyObjectAsServiceModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<LazyObject<IServiceForLazy>>();
        }
    }
}
