using System;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class DeclarativeServiceImpl1 : IDeclarativeService {
        public string Id { get; } = Guid.NewGuid().ToString();
        private readonly DeclarativeDependency _dependency;

        // Constructor that might be auto-selected by DI if no [Inject] or other rules apply
        public DeclarativeServiceImpl1() { }

        // Constructor for testing dependency injection into declaratively bound types
        [Inject]
        public DeclarativeServiceImpl1(DeclarativeDependency dependency) {
            _dependency = dependency;
        }

        public string Greet() => $"Hello from ServiceImpl1 ({Id})" + (_dependency != null ? " with " + _dependency.Describe() : "");
    }
} 