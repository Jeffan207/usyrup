using System;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    [Singleton]
    public class Butter : Identifiable {
        [Inject]
        public Butter() {}
    }
}
