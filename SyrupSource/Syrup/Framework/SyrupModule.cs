using UnityEngine;

namespace Syrup.Framework {
    /// <summary>
    /// Inherit class directly when defining a custom Syrup Module. 
    ///
    ///```
    /// public class ExampleSyrupModule : SyrupModule { ... }
    /// ```
    ///
    /// This class must be added to the same GameObject that holds the Syrup Component script. There
    /// is no limitation on the number of modules added to a single GameObject.
    /// Refer to <see href="https://github.com/Jeffan207/usyrup"/> for more details on how to implement Syrup Modules.
    /// </summary>
    [DefaultExecutionOrder(-901)]
    public abstract class SyrupModule : MonoBehaviour, ISyrupModule {}
}
