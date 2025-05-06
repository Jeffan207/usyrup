using System;

namespace Tests.Framework.TestData {
    public class DeclarativeServiceImpl2 : IDeclarativeService {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Greet() => $"Yo from ServiceImpl2 ({Id})";
    }
} 