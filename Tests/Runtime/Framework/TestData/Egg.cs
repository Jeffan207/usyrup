using System;
using Tests.Framework.TestData;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class Egg : Identifiable {

        public static readonly string SCRAMBLED_EGGS = "scrambled";
        public static readonly string SUNNY_SIDE_UP_EGGS = "sunny side up";

        public readonly string style;

        [Inject]
        public Egg() {
            this.style = SCRAMBLED_EGGS;
        }

        public Egg(string style) {
            this.style = style;
        }

    }
}

