using UnityEngine;
using System.Collections;
using Tests.Framework.TestData;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    /// <summary>
    /// Toast is always provided with every breakfast
    /// </summary>
    [SceneInjection(enabled: true)]
    public class Toast : MonoBehaviour {

        public Butter butter;

        [Inject]
        public void Init(Butter butter) {
            this.butter = butter;
        }
    }
}
