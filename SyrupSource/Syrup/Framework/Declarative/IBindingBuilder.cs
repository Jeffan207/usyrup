namespace Syrup.Framework.Declarative {
    /// <summary>
    ///     Builder interface for configuring a specific binding rule.
    /// </summary>
    /// <typeparam name="TService">The type being bound.</typeparam>
    public interface IBindingBuilder<TService> {
        /// <summary>
        ///     Specifies the implementation type for the binding rule.
        ///     When <see cref="TService" /> is requested , the container will create an instance of
        ///     <see cref="TImplementation" /> based on this rule.
        /// </summary>
        /// <typeparam name="TImplementation">
        ///     The concrete type to use, must be assignable to <see cref="TService" />.
        /// </typeparam>
        /// <returns>The builder for further chaining.</returns>
        IBindingBuilder<TService> To<TImplementation>() where TImplementation : TService;

        /// <summary>
        ///     Applies a name to the binding rule.
        ///     Dependencies requesting this binding will use <see cref="Syrup.Framework.Attributes.Named" />
        ///     with the same value as <see cref="name" />.
        /// </summary>
        /// <param name="name">The name for the binding rule.</param>
        /// <returns>The builder for further chaining.</returns>
        IBindingBuilder<TService> Named(string name);

        /// <summary>
        ///     Configures the binding rule to create/use a <see cref="Syrup.Framework.Attributes.Singleton" />
        ///     instance.
        ///     The container will ensure only one instance based on this rule exists per context lifetime.
        /// </summary>
        /// <returns>The builder for further chaining.</returns>
        IBindingBuilder<TService> AsSingleton();

        /// <summary>
        ///     Specifies a specific pre-existing instance for the binding rule.
        ///     When <see cref="TService" /> is requested, the container will always provide this exact
        ///     instance based on this rule.
        /// </summary>
        /// <param name="instance">The instance to use.</param>
        void ToInstance(TService instance);
    }
}
