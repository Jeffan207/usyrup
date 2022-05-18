using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class DuplicateProviderParametersModule : ISyrupModule {

        [Provides]
        public Breakfast ProvidesBreakfast(TastySyrup tastySyrup, Pancake pancake1, Pancake pancake2) {
            Pancake[] pancakes = new Pancake[] {pancake1, pancake2};
            return new Breakfast(tastySyrup, pancakes);
        }

        [Provides]
        public TastySyrup ProvidesTastySyrup() {
            return new TastySyrup();
        }

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

