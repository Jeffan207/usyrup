using Syrup.Framework;
using Syrup.Framework.Attributes;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;
using Tests.Framework.TestData.PropertyInjection;

namespace Tests.Framework.TestModules.PropertyInjection {
    public class PropertyModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<PropertyObjectNoConstructor>();
            binder.Bind<SimpleInheritedProperty>();
            binder.Bind<TastySyrup>();
            binder.Bind<LazyProperty>();
        }

        
        [Provides]
        [Named("CoolSyrup")]
        public TastySyrup ProvidesCoolSyrup() {
            return new TastySyrup();
        }
    }
}