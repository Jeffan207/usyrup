using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class SimpleProperty {
        [Inject]
        public TastySyrup SyrupProperty { get; set; }
    }
}