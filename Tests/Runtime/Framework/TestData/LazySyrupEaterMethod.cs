using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;

namespace Tests.Framework.TestData {
    public class LazySyrupEaterMethod : Identifiable {
        public LazyObject<TastySyrup> syrup;

        [Inject]
        public void Inject(LazyObject<TastySyrup> syrup) {
            this.syrup = syrup;
        }
    }
}
