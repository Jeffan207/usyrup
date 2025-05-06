using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DuplicateNamedProvidersModule : ISyrupModule {
        [Provides]
        [Named("MapleSyrup")]
        public TastySyrup ProvidesTastySyrup() => new();

        [Provides]
        [Named("MapleSyrup")]
        public TastySyrup ProvidesTastySyrupTwo() => new();
    }
}
