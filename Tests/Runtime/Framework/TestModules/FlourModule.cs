using Syrup.Framework;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;
using UnityEngine;

namespace Tests.Framework.TestModules {
    public class FlourModule : MonoBehaviour, ISyrupModule {
        [Provides]
        public Flour ProvidesFlour() => new();
    }
}
