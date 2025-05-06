using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeConstructorPrecedenceModule : ISyrupModule {
        public void Configure(IBinder binder) {
            // This binding for IDeclarativeService should be used by DeclarativeConcrete's [Inject] constructor
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl2>();

            // Bind DeclarativeConcrete to itself. SyrupInjector should pick its [Inject] constructor.
            binder.Bind<DeclarativeConcrete>().To<DeclarativeConcrete>();
        }
    }
}
