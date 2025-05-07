using System;

namespace Tests.Framework.TestData {
    public class DeclarativeServiceImpl1 : IDeclarativeService {
        public string Id { get; } = Guid.NewGuid().ToString();
        private readonly DeclarativeDependency _dependency;

        public DeclarativeServiceImpl1() { }

        public DeclarativeServiceImpl1(DeclarativeDependency dependency) =>
            _dependency = dependency;

        public string Greet() => $"Hello from ServiceImpl1 ({Id})" +
                                 (_dependency != null ? " with " + _dependency.Describe() : "");
    }
}
