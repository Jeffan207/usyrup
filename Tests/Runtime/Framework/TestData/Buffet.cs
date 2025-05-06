using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class Buffet : Identifiable {
        public Pancake pancake;
        public TastySyrup tastySyrup;

        [Inject]
        public Buffet() { }

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
