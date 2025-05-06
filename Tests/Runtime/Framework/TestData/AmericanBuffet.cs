using System;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class AmericanBuffet : Buffet {
        public Egg egg;

        [Inject]
        public AmericanBuffet() { }

        [Inject]
        public void AddEggs(Egg egg) {
            if (pancake == null || tastySyrup == null) {
                throw new ArgumentException(
                    "American's can't have breakfast without pancakes and syrup!");
            }

            this.egg = egg;
        }
    }
}
