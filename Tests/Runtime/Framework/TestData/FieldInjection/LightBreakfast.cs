using System.Collections;
using System.Collections.Generic;
using Syrup.Framework.Attributes;
using UnityEngine;

namespace Tests.Framework.TestData {
    public class LightBreakfast : Identifiable {

        [Inject]
        public OrangeJuice orangeJuice;

        public Egg egg;

        [Inject]
        public LightBreakfast() : base() { }

        [Inject]
        public void Init(Egg egg) {
            this.egg = egg;
        }
    }
}
