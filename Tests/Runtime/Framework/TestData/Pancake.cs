namespace Tests.Framework.TestData {
    public class Pancake : Identifiable {
        public readonly Flour flour;

        public Pancake(Flour flour) => this.flour = flour;
    }
}
