using Syrup.Framework;
using Syrup.Framework.Attributes;
using UnityEngine;

/// <summary>
/// This toast injects itself!
/// </summary>
namespace Tests.Framework.TestData {
    public class AutoToast : MonoBehaviour {
        public Butter butter;

        private void Start() {
            SyrupComponent.SyrupInjector.Inject(this);
        }

        [Inject]
        public void Init(Butter butter) {
            this.butter = butter;
        }
    }
}
