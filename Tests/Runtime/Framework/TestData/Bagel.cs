using Syrup.Framework.Attributes;
using UnityEngine;

namespace Tests.Framework.TestData {
    /// <summary>
    ///     Bagels have to be ordered separately
    /// </summary>
    [SceneInjection(false)]
    public class Bagel : MonoBehaviour {
        public Butter butter;

        [Inject]
        public void Init(Butter butter) {
            this.butter = butter;
        }
    }
}
