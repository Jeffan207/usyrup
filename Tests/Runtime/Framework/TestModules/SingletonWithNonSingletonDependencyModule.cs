using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class SingletonWithNonSingletonDependencyModule : ISyrupModule {
        [Provides]
        [Singleton]
        public Pancake ProvidesPancake(Flour flour) => new(flour);

        [Provides]
        public Flour ProvidesFlour() => new();
    }
}
