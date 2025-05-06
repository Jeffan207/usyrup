using System;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class LazySyrupEater : Identifiable {

        public readonly LazyObject<TastySyrup> syrup;

        [Inject]
        public LazySyrupEater(LazyObject<TastySyrup> syrup) : base() {
            this.syrup = syrup;
        }
    }
}

