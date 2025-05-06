using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class Waffle : Identifiable {
        public readonly Flour flour;
        public Butter butter;

        [Inject]
        public Waffle(Flour flour) => this.flour = flour;

        [Inject]
        public void Init(Butter butter) {
            this.butter = butter;
        }
    }
}
