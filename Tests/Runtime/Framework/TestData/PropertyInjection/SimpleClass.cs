using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class SimpleClass : Identifiable {
        
        [Inject]
        public SimpleClass() {}
    }
}