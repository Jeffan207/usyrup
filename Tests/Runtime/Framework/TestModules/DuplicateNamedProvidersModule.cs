using System;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DuplicateNamedProvidersModule : ISyrupModule {

        [Provides]
        [Named("MapleSyrup")]
        public TastySyrup ProvidesTastySyrup() {
            return new TastySyrup();
        }

        [Provides]
        [Named("MapleSyrup")]
        public TastySyrup ProvidesTastySyrupTwo() {
            return new TastySyrup();
        }
    }
}

