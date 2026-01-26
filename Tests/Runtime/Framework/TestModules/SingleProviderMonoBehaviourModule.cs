using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class SingleProviderMonoBehaviourModule : SyrupModule {
        [Provides]
        public TastySyrup ProvidesTastySyrup() {
            return new TastySyrup();
        }
    }
}
