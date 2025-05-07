namespace Tests.Framework.TestData.Declarative {
    public struct MyValueStructWithComplexConstructor : IMyValueService {
        public int X;
        public MyValueStructWithComplexConstructor(int x) { X = x; }
    }
} 