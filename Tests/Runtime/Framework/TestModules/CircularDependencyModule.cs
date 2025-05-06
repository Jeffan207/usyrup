using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class CircularDependencyModule : ISyrupModule {
        [Provides]
        public Egg ProvidesEgg(Waffle waffle) => new(Egg.SUNNY_SIDE_UP_EGGS);

        [Provides]
        public Waffle ProvidesWaffle(Egg egg) => new(new Flour());
    }
}
