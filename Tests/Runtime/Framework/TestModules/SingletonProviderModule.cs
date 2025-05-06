using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class SingletonProviderModule : ISyrupModule {
        [Provides]
        [Singleton]
        public Egg ProvidesEgg() => new();
    }
}
