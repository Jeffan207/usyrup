using System;
using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class LazyBreakfastClub : Identifiable {

        /// <summary>
        /// The LazySyrupEater is not Lazy, it just holds a lazy reference
        /// to a TastySyrup.
        /// </summary>        
        public LazySyrupEater lazySyrupEater;

        [Inject]
        public LazyBreakfastClub(LazySyrupEater lazySyrupEater) {
            this.lazySyrupEater = lazySyrupEater;
        }
    }
}

