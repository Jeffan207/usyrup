using System.Collections;
using System.Collections.Generic;
using Syrup.Framework.Attributes;
using UnityEngine;

namespace Tests.Framework.TestData {
    public class CanadianSyrup : Identifiable {

        [Inject]
        [Named("MapleSyrup")]
        public TastySyrup tastySyrup;

        [Inject]
        public CanadianSyrup() : base() { }
    }
}