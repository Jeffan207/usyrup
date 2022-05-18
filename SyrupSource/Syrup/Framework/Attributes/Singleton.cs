using System;

namespace Syrup.Framework.Attributes {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Singleton : Attribute { }
}
