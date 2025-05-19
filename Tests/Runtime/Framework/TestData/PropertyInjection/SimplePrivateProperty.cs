using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class SimplePrivateProperty {
        
        [Inject]
        private TastySyrup SyrupProperty { get; set; }

        public TastySyrup GetSyrup() {
            return SyrupProperty;
        }
    }
}