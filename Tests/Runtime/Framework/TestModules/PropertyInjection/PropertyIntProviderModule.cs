using Syrup.Framework;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestModules.PropertyInjection {
    public class PropertyProviderIntModule : ISyrupModule {

        [Provides]
        public int ProvidesInt() {
            return 75;
        }
    }
}