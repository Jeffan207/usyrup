using System.Collections;
using System.Collections.Generic;
using Syrup.Framework.Attributes;
using UnityEngine;

namespace Tests.Framework.TestData {
    public class PrivateBreakfast : Identifiable {

        [Inject]
        private OrangeJuice orangeJuice;

        public Egg egg;

        [Inject]
        public PrivateBreakfast() : base() { }

        [Inject]
        private void Init(Egg egg) {
            this.egg = egg;
        }

        public Egg GetEgg() {
            return egg;
        }

        public OrangeJuice GetOrangeJuice() {
            return orangeJuice;
        }
    }


}
