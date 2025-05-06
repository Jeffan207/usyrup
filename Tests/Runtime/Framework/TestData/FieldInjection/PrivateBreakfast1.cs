using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class PrivateBreakfast : Identifiable {
        [Inject]
        private OrangeJuice orangeJuice;

        public Egg egg;

        [Inject]
        public PrivateBreakfast() { }

        [Inject]
        private void Init(Egg egg) {
            this.egg = egg;
        }

        public Egg GetEgg() => egg;

        public OrangeJuice GetOrangeJuice() => orangeJuice;
    }
}
