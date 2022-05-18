using System;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class SingletonWithNonSingletonDependencyModule : ISyrupModule {

        [Provides]
        [Singleton]
        public Pancake ProvidesPancake(Flour flour) {
            return new Pancake(flour);
        }

        [Provides]
        public Flour ProvidesFlour() {
            return new Flour();
        }
    }
}

