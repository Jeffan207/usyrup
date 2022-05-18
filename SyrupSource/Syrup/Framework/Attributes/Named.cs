using System;

namespace Syrup.Framework.Attributes {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class Named : Attribute {

        public readonly string name;

        public Named(string name) {
            this.name = name;
        }
    }
}
