using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class TwoDependentProvidersModule : ISyrupModule {

        [Provides]
        public Pancake ProvidesPancake(Flour flour) {
            return new Pancake(flour);
        }

        [Provides]
        public Flour ProvidesFlour() {
            return new Flour();
        }
    }
}

