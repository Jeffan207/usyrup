using System;

namespace Syrup.Framework.Attributes {
    /// <summary>
    /// Attribute that signals to USyrup if this MonoBehaviour should be injected on scene load. By default, all
    /// MonoBehaviours are treated as if SceneInjection.enabled = true. By disabling scene injection the MonoBehaviour
    /// will need to be injected via on-demand injection via SyrupInjector.Get<Type>() or SyrupInjector.Inject(this).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneInjection : Attribute {

        public readonly bool enabled;

        public SceneInjection(bool enabled = true) {
            this.enabled = enabled;
        }

    }
}
