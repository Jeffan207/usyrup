using System;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class Buffet : Identifiable {

        public Pancake pancake = null;
        public TastySyrup tastySyrup = null;

        [Inject]
        public Buffet() {

        }

        [Inject]
        public void SetupPancakes(Pancake pancake) {
            this.pancake = pancake;
        }

        [Inject]
        public void SetupSyrup(TastySyrup tastySyrup) {
            this.tastySyrup = tastySyrup;
        }
    }
}

