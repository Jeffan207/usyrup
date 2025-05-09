using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeConstructorPrecedenceModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl2>();
            binder.Bind<DeclarativeConcrete>().To<DeclarativeConcrete>();
        }
    }
}
