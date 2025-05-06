using System;
using UnityEngine;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class FlourModule : MonoBehaviour, ISyrupModule {

        [Provides]
        public Flour ProvidesFlour() {
            return new Flour();
        }
    }
}

