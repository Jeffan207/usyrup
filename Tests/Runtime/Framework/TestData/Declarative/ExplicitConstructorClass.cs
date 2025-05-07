namespace Tests.Framework.TestData {
    public class ExplicitConstructorClass {
        public string Message { get; }
        public ExplicitConstructorClass(DeclarativeDependency dep) {
            Message = $"Constructed with {dep.Describe()}";
        }
    }
} 