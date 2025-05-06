using System;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class LazyProviderParamModule : ISyrupModule {

        [Provides]
        public LazySyrupEater ProvidesLazySyrupEater(LazyObject<TastySyrup> tastySyrup) {
            return new LazySyrupEater(tastySyrup);
        }

        [Provides]
        public TastySyrup ProvidesTastySyrup() {
            return new TastySyrup();
        }
    }
}

