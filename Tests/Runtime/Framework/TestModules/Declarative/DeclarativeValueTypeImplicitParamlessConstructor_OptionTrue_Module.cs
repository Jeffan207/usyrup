using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData.Declarative;

namespace Tests.Framework.TestModules {
    public class DeclarativeValueTypeImplicitParameterlessConstructorOptionTrueModule : ISyrupModule {
        public void Configure(IBinder binder) {
            binder.Bind<MyValueStruct>(); // MyValueStruct has an implicit parameterless constructor
        }
    }
}
