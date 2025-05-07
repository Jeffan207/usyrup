namespace Tests.Framework.TestData {
    public class NoPublicConstructorClass {
        private NoPublicConstructorClass() { }

        public static NoPublicConstructorClass Create() => new NoPublicConstructorClass();
        public string Test() => "Instance exists";
    }
} 