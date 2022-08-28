using System;

namespace Syrup.Framework.Attributes {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field)]
    public class Inject : Attribute {}
}