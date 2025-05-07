using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData.Declarative;

namespace Tests.Framework.TestModules {
    public class DeclarativeValueTypeNoSuitableConstructorModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<IMyValueService>().To<MyValueStructWithComplexConstructor>();
        }
    }
}
