using System;

namespace Syrup.Framework.Declarative {
    internal class Binding {
        public Type BoundService { get; }
        public Type ImplementationType { get; internal set; }
        public object Instance { get; internal set; }
        public string Name { get; internal set; }
        public bool IsSingleton { get; internal set; }

        public Binding(Type boundService) {
            BoundService = boundService ?? throw new ArgumentNullException(nameof(boundService));
        }
    }
}
