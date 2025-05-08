using System;

namespace Tests.Framework.TestData {
    public class DeclarativeServiceImpl1 : IDeclarativeService {
        public string Id { get; } = Guid.NewGuid().ToString();
        private readonly DeclarativeDependency _dependency;

        public static bool WasInstantiated_ServiceImpl1 = false;

        public DeclarativeServiceImpl1() {
            WasInstantiated_ServiceImpl1 = true;
        }

        public DeclarativeServiceImpl1(DeclarativeDependency dependency) {
            _dependency = dependency;
            WasInstantiated_ServiceImpl1 = true;
        }

        public string Greet() => $"Hello from ServiceImpl1 ({Id})" +
                                 (_dependency != null ? " with " + _dependency.Describe() : "");
    }
}
