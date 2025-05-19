using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules.PropertyInjection {
    public class PropertyLargeIntModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<int>().ToInstance(1000);
        }
    }
}