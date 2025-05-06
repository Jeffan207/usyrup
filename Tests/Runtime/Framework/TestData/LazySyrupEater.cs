using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;

namespace Tests.Framework.TestData {
    public class LazySyrupEater : Identifiable {
        public readonly LazyObject<TastySyrup> syrup;

        [Inject]
        public LazySyrupEater(LazyObject<TastySyrup> syrup) => this.syrup = syrup;
    }
}
