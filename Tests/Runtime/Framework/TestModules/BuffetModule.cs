using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class BuffetModule : ISyrupModule {
        [Provides]
        public Buffet ProvidesBuffet() => new();
    }
}
