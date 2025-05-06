using System;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class LazySyrupEaterMethod : Identifiable {

        public LazyObject<TastySyrup> syrup;

        [Inject]
        public void Inject(LazyObject<TastySyrup> syrup) {
            this.syrup = syrup;
        }
    }
}

