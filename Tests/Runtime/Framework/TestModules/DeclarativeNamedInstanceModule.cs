using Syrup.Framework;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DeclarativeNamedInstanceModule : ISyrupModule {
        public readonly IDeclarativeService Instance1 = new DeclarativeServiceImpl1();
        public readonly IDeclarativeService Instance2 = new DeclarativeServiceImpl2();

        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().Named("Instance1").ToInstance(Instance1);
            binder.Bind<IDeclarativeService>().Named("Instance2").ToInstance(Instance2);
        }
    }
} 
