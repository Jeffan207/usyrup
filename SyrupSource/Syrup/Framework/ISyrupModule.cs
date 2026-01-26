using System;
using Syrup.Framework.Declarative;

namespace Syrup.Framework {
    
    /// <summary>
    /// NOTE: Prefer extending <see cref="SyrupModule"/> directly instead of directly implementing this interface!
    /// The SyrupModule MonoBehaviour is configured to run with the appropriate script execution order, otherwise,
    /// inject in awake won't function correctly.
    /// </summary>
    public interface ISyrupModule {
        /// <summary>
        ///     Configure the module using a binder.
        ///     This method is called when the module is loaded along with the provider methods.
        /// </summary>
        /// <param name="binder">The binder to use for configuring the module.</param>
        void Configure(IBinder binder) { }
    }
}
