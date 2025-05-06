namespace Tests.Framework.TestData {
    public class NoPublicConstructorClass {
        private NoPublicConstructorClass() { }

        public static NoPublicConstructorClass Create() => new();
        public string Test() => "Instance exists";
    }
}
