namespace Tests.Framework.TestData {
    public class Breakfast : Identifiable {
        public readonly TastySyrup tastySyrup;
        public readonly Pancake[] pancakes;

        public Breakfast(TastySyrup tastySyrup, params Pancake[] pancakes) {
            this.tastySyrup = tastySyrup;
            this.pancakes = pancakes;
        }
    }
}
