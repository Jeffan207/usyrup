using Syrup.Framework.Declarative;

namespace Syrup.Framework {
    public interface ISyrupModule {
        /// <summary>
        ///     Configure the module using a binder.
        ///     This method is called when the module is loaded along with the provider methods.
        /// </summary>
        /// <param name="binder">The binder to use for configuring the module.</param>
        void Configure(IBinder binder) { }
    }
}
