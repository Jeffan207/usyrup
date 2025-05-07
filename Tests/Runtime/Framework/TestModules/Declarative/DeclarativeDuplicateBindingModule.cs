using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeDuplicateBindingModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().Named("DuplicateName").To<DeclarativeServiceImpl1>();
            binder.Bind<IDeclarativeService>().Named("DuplicateName").To<DeclarativeServiceImpl2>();
        }
    }
}
