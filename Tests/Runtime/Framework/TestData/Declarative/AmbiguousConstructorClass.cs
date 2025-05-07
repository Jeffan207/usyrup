namespace Tests.Framework.TestData {
    public class AmbiguousConstructorClass {
        public string ChosenConstructor { get; }

        public AmbiguousConstructorClass(int val) => ChosenConstructor = "int Ctor";

        public AmbiguousConstructorClass(string val) => ChosenConstructor = "string Ctor";
    }
}
