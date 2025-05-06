using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeNoPublicConstructorModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<NoPublicConstructorClass>().To<NoPublicConstructorClass>();
        }
    }
}
