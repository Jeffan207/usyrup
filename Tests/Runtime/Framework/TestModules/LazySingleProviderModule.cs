using Syrup.Framework;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class LazySingleProviderModule : ISyrupModule {
        // This is an invalid module!
        [Provides]
        public LazyObject<TastySyrup> ProvidesTastySyrup() => new();
    }
}
