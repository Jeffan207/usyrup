using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;

namespace Tests.Framework.TestData.PropertyInjection {
    public class LazyProperty {
        [Inject]
        public LazyObject<SimpleClass> LazySimpleClass { get; set; }
    }
}