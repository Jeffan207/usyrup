using System;
using Tests.Framework.TestData;
using Syrup.Framework.Attributes;


namespace Tests.Framework.TestData {
    public class OrangeJuice : Identifiable {

        [Inject]
        public Orange orange;

        [Inject]
        public OrangeJuice(): base() { }
    }
}