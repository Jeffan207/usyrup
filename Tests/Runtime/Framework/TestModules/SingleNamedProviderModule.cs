using System;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class SingleNamedProviderModule : ISyrupModule {

        [Provides]
        [Named("MapleSyrup")]
        public TastySyrup ProvidesTastySyrup() {
            return new TastySyrup();
        }
    }
}

