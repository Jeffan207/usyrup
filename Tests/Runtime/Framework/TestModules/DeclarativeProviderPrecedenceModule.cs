using Syrup.Framework;
using Syrup.Framework.Attributes;
using Syrup.Framework.Declarative;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    // For testing Provider vs Declarative precedence
    public class DeclarativeProviderPrecedenceModule : ISyrupModule {
        public const string ProvidedId = "FROM_PROVIDER";

        [Provides]
        public IDeclarativeService ProvideService() =>
            new DeclarativeServiceImpl1WithCustomId(ProvidedId);

        public void Configure(IBinder binder) {
            // This binding should be ignored because a Provider exists for IDeclarativeService
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl2>();
        }
    }

    // Helper class for this test
    public class DeclarativeServiceImpl1WithCustomId : IDeclarativeService {
        public string Id { get; }
        public DeclarativeServiceImpl1WithCustomId(string id) => Id = id;
        public string Greet() => $"Hello from Custom ID Service ({Id})";
    }
}
