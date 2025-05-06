using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class StateBrunch : Identifiable {
        [Inject]
        public Egg egg;

        public Butter butter;

        [Inject]
        public StateBrunch() { }

        [Inject]
        public void InitStateBrunch(Butter butter) {
            this.butter = butter;
        }
    }
}
