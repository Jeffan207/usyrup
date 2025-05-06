using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class Omelette : Identifiable {
        public readonly Egg egg;

        [Inject] //It's a one egg omelette
        public Omelette(Egg egg) => this.egg = egg;
    }
}
