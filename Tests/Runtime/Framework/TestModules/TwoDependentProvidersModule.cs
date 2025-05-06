using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class TwoDependentProvidersModule : ISyrupModule {
        [Provides]
        public Pancake ProvidesPancake(Flour flour) => new(flour);

        [Provides]
        public Flour ProvidesFlour() => new();
    }
}
