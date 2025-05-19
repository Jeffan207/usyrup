using Syrup.Framework;
using Syrup.Framework.Attributes;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;
using Tests.Framework.TestData.PropertyInjection;

namespace Tests.Framework.TestModules.PropertyInjection {
    public class SingletonPropertyModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<TastySyrup>().AsSingleton();
            binder.Bind<SimpleProperty>();
        }
        
        
    }
}