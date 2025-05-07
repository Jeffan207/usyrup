using Syrup.Framework;
using Syrup.Framework.Declarative;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestModules {
    // For testing Provider and Declarative duplication.
    public class DeclarativeProviderDuplicateModule : ISyrupModule {
        public const string ProvidedId = "FROM_PROVIDER";

        [Provides]
        public IDeclarativeService ProvideService() {
            return new DeclarativeServiceImpl1WithCustomId(ProvidedId);
        }

        public void Configure(IBinder binder) {
            binder.Bind<IDeclarativeService>().To<DeclarativeServiceImpl2>();
        }
    }

    // Helper class for this test
    public class DeclarativeServiceImpl1WithCustomId : IDeclarativeService {
        public string Id { get; }
        public DeclarativeServiceImpl1WithCustomId(string id) { Id = id; }
        public string Greet() => $"Hello from Custom ID Service ({Id})";
    }
}
