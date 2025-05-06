using System.Collections.Generic;

namespace Syrup.Framework.Declarative {
    internal class Binder : IBinder {
        private readonly List<Binding> _bindings = new();

        public IBindingBuilder<TService> Bind<TService>() {
            var binding = new Binding(typeof(TService));
            _bindings.Add(binding);
            return new BindingBuilder<TService>(binding);
        }

        public IBindingBuilder<TService> Bind<TService, TImplementation>()
            where TImplementation : TService {
            var binding = new Binding(typeof(TService)) {
                ImplementationType = typeof(TImplementation)
            };
            _bindings.Add(binding);
            return new BindingBuilder<TService>(binding);
        }

        internal IReadOnlyCollection<Binding> GetBindings() => _bindings.AsReadOnly();
    }
}
