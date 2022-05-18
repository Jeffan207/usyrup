using System;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class Pancake : Identifiable {

        public readonly Flour flour;

        public Pancake(Flour flour) : base() {
            this.flour = flour;
        }
    }
}

