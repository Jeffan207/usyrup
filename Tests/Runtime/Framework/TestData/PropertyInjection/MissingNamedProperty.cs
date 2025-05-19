using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class MissingNamedProperty {
        [Inject]
        [Named("MissingSyrup")] // Should not be provided anywhere
        public TastySyrup SyrupProperty { get; set; }
        
        [Inject]
        public MissingNamedProperty() {}
    }
}