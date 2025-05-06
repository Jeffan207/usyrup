using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;

namespace Tests.Framework.TestData {
    public class LazyEggEater : Identifiable {
        [Inject]
        public LazyObject<Egg> egg;
    }
}
