using System;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class LazyEggEater : Identifiable {
        [Inject]
        public LazyObject<Egg> egg;
    }
}

