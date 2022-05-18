using System;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class SingleProviderModule : ISyrupModule {

        [Provides]
        public TastySyrup ProvidesTastySyrup() {
            return new TastySyrup();
        }
    }
}

