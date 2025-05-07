using System;

namespace Syrup.Framework.Declarative {
    internal class BindingBuilder<TService> : IBindingBuilder<TService> {
        private readonly Binding _binding;

        public BindingBuilder(Binding binding) => _binding = binding;

        public IBindingBuilder<TService> To<TImplementation>() where TImplementation : TService {
            if (_binding.Instance != null) {
                throw new InvalidOperationException(
                    "Cannot set implementation type after ToInstance() has been called.");
            }

            _binding.ImplementationType = typeof(TImplementation);
            return this;
        }

        public IBindingBuilder<TService> Named(string name) {
            _binding.Name = name;
            return this;
        }

        public IBindingBuilder<TService> AsSingleton() {
            _binding.IsSingleton = true;
            return this;
        }

        public void ToInstance(TService instance) {
            // If ToInstance is called, it takes precedence.
            // Clear any potentially pre-filled ImplementationType from a default self-binding.
            _binding.ImplementationType = null;

            _binding.Instance = instance;
            // ToInstance implies a singleton for that specific instance.
            _binding.IsSingleton = true;
        }
    }
}
