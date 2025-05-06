using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeInstanceModule : ISyrupModule {
        public readonly DeclarativeServiceImpl1 MyInstance = new();

        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().ToInstance(MyInstance);
        }
    }
}
