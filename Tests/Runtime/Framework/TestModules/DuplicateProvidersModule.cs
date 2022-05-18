using System;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DuplicateProvidersModule : ISyrupModule {

        [Provides]
        public TastySyrup ProvidesTastySyrup() {
            return new TastySyrup();
        }

        [Provides]
        public TastySyrup ProvidesTastySyrupTwo() {
            return new TastySyrup();
        }
    }
}

