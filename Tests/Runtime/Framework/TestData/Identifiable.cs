using System;
namespace Tests.Framework.TestData {
    //Helper class to allow us to ID all the generated test objects
    public class Identifiable {

        public readonly string id;

        public Identifiable() {
            id = Guid.NewGuid().ToString();
        }
    }
}
