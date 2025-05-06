using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class TwoNamedProvidersModule : ISyrupModule {
        [Provides]
        [Named("FluffyPancake")]
        public Pancake ProvidesPancake([Named("WholeGrainFlour")] Flour flour) => new(flour);

        [Provides]
        [Named("WholeGrainFlour")]
        public Flour ProvidesFlour() => new();
    }
}
