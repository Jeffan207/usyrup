using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;
using UnityEngine;

namespace Tests.Framework.TestModules {
    public class ProvidedEggModule : MonoBehaviour, ISyrupModule {
        [Provides]
        public Egg ProvidesEgg() => new(Egg.SUNNY_SIDE_UP_EGGS);
    }
}
