using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class ProviderWithMissingDependencyModule : ISyrupModule {

        [Provides]
        public Pancake ProvidesPancake(Flour flour) {
            return new Pancake(flour);
        }
    }
}

