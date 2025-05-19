using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class PropertyFromConstructorInjectedType {
        [Inject]
        public SimpleClass SimpleClass { get; set; }
    }
}