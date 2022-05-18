using System;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class AmericanBuffet : Buffet {

        public Egg egg;

        [Inject]
        public AmericanBuffet() {

        }

        [Inject]
        public void AddEggs(Egg egg) {
            if (this.pancake == null || this.tastySyrup == null) {
                throw new ArgumentException("American's can't have breakfast without pancakes and syrup!");
            }

            this.egg = egg;
        }
    }
}

