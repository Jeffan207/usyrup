using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    [Singleton]
    public class EnglishMuffin : Identifiable {
        public Butter butter;

        [Inject]
        public void Init(Butter butter) {
            this.butter = butter;
        }
    }
}
