using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;
using Tests.Framework.TestData.PropertyInjection;

namespace Tests.Framework.TestModules.PropertyInjection {
    public class MissingNamedPropertyModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<MissingNamedProperty>();
        }
    }
}