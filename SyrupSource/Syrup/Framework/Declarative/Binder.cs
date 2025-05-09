using System;
using System.Collections.Generic;

namespace Syrup.Framework.Declarative {
    internal class Binder : IBinder {
        private readonly List<Binding> _bindings = new();

        public IBindingBuilder<TService> Bind<TService>() {
            Type serviceType = typeof(TService);
            Binding newBinding = new Binding(serviceType);

            // Default to self-binding if TService is a concrete type
            if (!serviceType.IsAbstract && !serviceType.IsInterface) {
                newBinding.ImplementationType = serviceType;
            }

            _bindings.Add(newBinding);
            return new BindingBuilder<TService>(newBinding);
        }

        public IBindingBuilder<TService> Bind<TService, TImplementation>()
            where TImplementation : TService {
            Binding binding = new Binding(typeof(TService)) {
                ImplementationType = typeof(TImplementation)
            };
            _bindings.Add(binding);
            return new BindingBuilder<TService>(binding);
        }

        internal IReadOnlyCollection<Binding> GetBindings() => _bindings.AsReadOnly();
    }
}
