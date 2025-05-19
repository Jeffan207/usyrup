using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class SimpleInheritedProperty : SimpleProperty {
        
        [Inject]
        public SimpleClass SimpleClass { get; set; }
        
    }
}