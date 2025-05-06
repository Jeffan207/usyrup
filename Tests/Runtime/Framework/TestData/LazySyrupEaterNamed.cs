using System;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class LazySyrupEaterNamed : Identifiable {
       
        public LazyObject<TastySyrup> syrup;

        [Inject]
        public LazySyrupEaterNamed([Named("MapleSyrup")] LazyObject<TastySyrup> syrup) {
            this.syrup = syrup;
        }
    }
}

