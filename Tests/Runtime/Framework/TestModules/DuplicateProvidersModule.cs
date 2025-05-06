using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DuplicateProvidersModule : ISyrupModule {
        [Provides]
        public TastySyrup ProvidesTastySyrup() => new();

        [Provides]
        public TastySyrup ProvidesTastySyrupTwo() => new();
    }
}
