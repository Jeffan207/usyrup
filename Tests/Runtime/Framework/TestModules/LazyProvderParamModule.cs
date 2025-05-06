using Syrup.Framework;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class LazyProviderParamModule : ISyrupModule {
        [Provides]
        public LazySyrupEater ProvidesLazySyrupEater(LazyObject<TastySyrup> tastySyrup) =>
            new(tastySyrup);

        [Provides]
        public TastySyrup ProvidesTastySyrup() => new();
    }
}
