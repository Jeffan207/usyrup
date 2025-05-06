using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    [Singleton]
    public class Butter : Identifiable {
        [Inject]
        public Butter() { }
    }
}
