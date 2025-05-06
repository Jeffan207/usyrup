using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;

namespace Tests.Framework.TestData {
    public class LazySyrupEaterField : Identifiable {
        [Inject]
        public readonly LazyObject<TastySyrup> syrup;
    }
}
