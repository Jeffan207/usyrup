using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class LightBreakfast : Identifiable {
        [Inject]
        public OrangeJuice orangeJuice;

        public Egg egg;

        [Inject]
        public LightBreakfast() { }

        [Inject]
        public void Init(Egg egg) {
            this.egg = egg;
        }
    }
}
