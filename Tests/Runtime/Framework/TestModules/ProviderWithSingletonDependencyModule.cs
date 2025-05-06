using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class ProviderWithSingletonDependencyModule : ISyrupModule {
        [Provides]
        public Pancake ProvidesPancake(Flour flour) => new(flour);

        [Provides]
        [Singleton]
        public Flour ProvidesFlour() => new();
    }
}
