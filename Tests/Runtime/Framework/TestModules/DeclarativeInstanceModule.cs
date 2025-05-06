using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeInstanceModule : ISyrupModule {
        public readonly DeclarativeServiceImpl1 MyInstance = new DeclarativeServiceImpl1();
        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().ToInstance(MyInstance);
        }
    }
}
