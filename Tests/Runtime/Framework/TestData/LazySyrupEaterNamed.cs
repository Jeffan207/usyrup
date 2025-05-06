using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;

namespace Tests.Framework.TestData {
    public class LazySyrupEaterNamed : Identifiable {
        public LazyObject<TastySyrup> syrup;

        [Inject]
        public LazySyrupEaterNamed([Named("MapleSyrup")] LazyObject<TastySyrup> syrup) =>
            this.syrup = syrup;
    }
}
