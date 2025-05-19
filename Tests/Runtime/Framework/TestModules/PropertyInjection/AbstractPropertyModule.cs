using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;
using Tests.Framework.TestData.PropertyInjection;

namespace Tests.Framework.TestModules.PropertyInjection {
    public class AbstractPropertyModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<SimpleClass>();
            binder.Bind<TastySyrup>();
            binder.Bind<SimpleInheritedProperty>();
            binder.Bind<SimpleProperty>().To<SimpleInheritedProperty>();
        }
    }
}