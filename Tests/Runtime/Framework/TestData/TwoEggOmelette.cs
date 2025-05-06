using System;
using Tests.Framework.TestData;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class TwoEggOmelette : Identifiable {

        public readonly Egg egg1;
        public readonly Egg egg2;

        [Inject] //It's a two egg omelette!!
        public TwoEggOmelette(Egg egg1, Egg egg2) {
            this.egg1 = egg1;
            this.egg2 = egg2;
        }

    }
}

