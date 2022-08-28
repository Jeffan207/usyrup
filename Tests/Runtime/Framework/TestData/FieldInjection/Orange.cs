using System;
using Syrup.Framework.Attributes;
using Tests.Framework.TestData;

namespace Tests.Framework.TestData {
    public class Orange : Identifiable {

        [Inject]
        public Orange(): base() { }

    }
}

