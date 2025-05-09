using System;

namespace Tests.Framework.TestData {
    public class DeclarativeDependency {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Describe() => $"Dependency ({Id})";
    }
}
