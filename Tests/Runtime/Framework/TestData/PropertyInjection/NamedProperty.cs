using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class NamedProperty {
        [Inject]
        [Named("CoolSyrup")]
        public TastySyrup SyrupProperty { get; set; }
        
        [Inject]
        public NamedProperty() {}
    }
}