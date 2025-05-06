using Syrup.Framework;
using Syrup.Framework.Attributes;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    /// <summary>
    ///     This module tests dependency injection into a specific constructor.
    ///     It uses Configure for simple bindings (the dependency) and [Provides]
    ///     to force the injector to use the specific constructor for the service,
    ///     overriding default constructor selection rules for that specific binding.
    /// </summary>
    public class DeclarativeInjectSpecificCtorHybridModule : ISyrupModule {
        // Use [Provides] ONLY for the complex/specific construction case
        [Provides]
        public IDeclarativeService ProvideServiceWithDependency(DeclarativeDependency dep) =>
            // Explicitly call the constructor we want to test injection for
            new DeclarativeServiceImpl1(dep);

        // Use Configure for standard/simple bindings
        public void Configure(IBinder binder) {
            binder.Bind<DeclarativeDependency>().To<DeclarativeDependency>();
        }
    }
}
