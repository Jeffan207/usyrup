using System;
using System.Collections.Generic;

namespace Syrup.Framework.Declarative {
    internal class Binder : IBinder {
        private readonly HashSet<Binding> _bindings = new();

        public IBindingBuilder<TService> Bind<TService>() => throw new NotImplementedException();

        public IBindingBuilder<TService> Bind<TService, TImplementation>()
            where TImplementation : TService => throw new NotImplementedException();

        internal IReadOnlyCollection<Binding> Bindings() => _bindings;
    }
}
