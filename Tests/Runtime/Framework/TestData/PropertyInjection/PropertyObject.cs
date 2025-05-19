using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class PropertyObject {
        
        [Inject]
        public SimpleClass SimpleClass { get; set; }
        
        [Inject]
        public PropertyObject() {}
        
    }
}