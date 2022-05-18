using System;

namespace Syrup.Framework.Attributes {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class Inject : Attribute {}
}