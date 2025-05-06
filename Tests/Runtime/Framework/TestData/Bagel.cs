using UnityEngine;
using System.Collections;
using Tests.Framework.TestData;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    /// <summary>
    /// Bagels have to be ordered separately
    /// </summary>
    [SceneInjection(enabled: false)]
    public class Bagel : MonoBehaviour {

        public Butter butter;

        [Inject]
        public void Init(Butter butter) {
            this.butter = butter;
        }
    }
}
