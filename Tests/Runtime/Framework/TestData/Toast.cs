using Syrup.Framework.Attributes;
using UnityEngine;

namespace Tests.Framework.TestData {
    /// <summary>
    ///     Toast is always provided with every breakfast
    /// </summary>
    [SceneInjection]
    public class Toast : MonoBehaviour {
        public Butter butter;

        [Inject]
        public void Init(Butter butter) {
            this.butter = butter;
        }
    }
}
