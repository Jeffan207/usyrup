using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class PropertyObjectNoConstructor {
        
        [Inject]
        public SimpleClass SimpleClass { get; set; }
        
    }
}