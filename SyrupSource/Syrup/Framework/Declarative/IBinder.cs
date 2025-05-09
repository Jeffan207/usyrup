namespace Syrup.Framework.Declarative {
    /// <summary>
    ///     Provides a mechanism for configuring bindings between service types and their implementations.
    /// </summary>
    public interface IBinder {
        /// <summary>
        ///     Starts a binding configuration rule for the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type to bind (often an interface or base class).</typeparam>
        /// <returns>An <see cref="IBindingBuilder{TService}" /> to continue the configuration.</returns>
        IBindingBuilder<TService> Bind<TService>();

        /// <summary>
        ///     Binds a service type to its implementation.
        /// </summary>
        /// <typeparam name="TService">The type to bind (often an interface or base class).</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>An <see cref="IBindingBuilder{TService}" /> to continue the configuration.</returns>
        IBindingBuilder<TService> Bind<TService, TImplementation>()
            where TImplementation : TService;
    }
}
