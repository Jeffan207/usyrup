using System;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class DeclarativeConcrete {
        public string Id { get; } = Guid.NewGuid().ToString();
        public IDeclarativeService Service { get; private set; }
        public string Message { get; }

        public DeclarativeConcrete() {
            Message = "Default Ctor";
        }

        [Inject]
        public DeclarativeConcrete(IDeclarativeService service) {
            Service = service;
            Message = "Injected Ctor";
        }

        public string Introduce() => $"Concrete {Id} says: {Message}, Service: {(Service != null ? Service.Greet() : "null")}";
    }
} 