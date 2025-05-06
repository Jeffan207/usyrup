using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DuplicateProviderParametersModule : ISyrupModule {
        [Provides]
        public Breakfast ProvidesBreakfast(
            TastySyrup tastySyrup, Pancake pancake1, Pancake pancake2
        ) {
            var pancakes = new[] { pancake1, pancake2 };
            return new Breakfast(tastySyrup, pancakes);
        }

        [Provides]
        public TastySyrup ProvidesTastySyrup() => new();

        [Provides]
        public Pancake ProvidesPancake(Flour flour) => new(flour);

        [Provides]
        public Flour ProvidesFlour() => new();
    }
}
