using System;

namespace Syrup.Framework.Attributes {
    [AttributeUsage(AttributeTargets.Method | 
                    AttributeTargets.Constructor | 
                    AttributeTargets.Field | 
                    AttributeTargets.Property)]
    public class Inject : Attribute {}
}