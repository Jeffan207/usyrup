using System;
using Syrup.Framework.Attributes;

namespace Syrup.Framework.Declarative {
    internal class Binding {
        public Type Bound { get; }
        public Named Named { get; internal set; }
        public bool IsSingleton { get; internal set; }
        // TODO: Implement more things if needed.
    }
}
