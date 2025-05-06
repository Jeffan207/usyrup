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
            if (_binding.ImplementationType != null) {
                throw new InvalidOperationException(
                    "Cannot use ToInstance() after specifying an implementation type. Use Bind<TService>() instead of Bind<TService, TImplementation>().");
            }

            _binding.Instance = instance;
            _binding.IsSingleton = true;
        }
    }
}
