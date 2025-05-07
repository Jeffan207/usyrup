using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeIncompleteBindingModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>(); // Missing .To<T>() or .ToInstance()
        }
    }
}
