using System;
using Syrup.Framework;
using UnityEngine;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    public class ProvidedEggModule : MonoBehaviour, ISyrupModule {

        [Provides]
        public Egg ProvidesEgg() {
            return new Egg(Egg.SUNNY_SIDE_UP_EGGS);
        }

    }
}

