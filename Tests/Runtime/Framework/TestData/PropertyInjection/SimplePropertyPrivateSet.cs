using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class SimplePropertyPrivateSet {
        
        [Inject]
        public TastySyrup SyrupProperty { get; private set; }
    }
}