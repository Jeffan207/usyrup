using System;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class CircularDependencyModule : ISyrupModule {

        [Provides]
        public Egg ProvidesEgg(Waffle waffle) {
            return new Egg(Egg.SUNNY_SIDE_UP_EGGS);
        }

        [Provides]
        public Waffle ProvidesWaffle(Egg egg) {
            return new Waffle(new Flour());
        }

    }
}

