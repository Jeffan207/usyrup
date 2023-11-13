using System;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class LazySyrupEaterField : Identifiable {
        [Inject]
        public readonly LazyObject<TastySyrup> syrup;
    }
}

