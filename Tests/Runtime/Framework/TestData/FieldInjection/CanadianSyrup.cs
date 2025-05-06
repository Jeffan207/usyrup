using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class CanadianSyrup : Identifiable {
        [Inject]
        [Named("MapleSyrup")]
        public TastySyrup tastySyrup;

        [Inject]
        public CanadianSyrup() { }
    }
}
