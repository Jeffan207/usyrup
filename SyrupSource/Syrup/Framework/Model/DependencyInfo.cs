using System;
using System.Collections.Generic;
using System.Reflection;

namespace Syrup.Framework.Model {

    /// <summary>
    /// Stores some basic info about a particular dependency. Helps identify whether the dependency
    /// should be provided via Constructor or a Provider method and stores relevant information depending on which.
    /// Not all fields will be used by either type.
    /// </summary>
    internal struct DependencyInfo {
        public DependencySource DependencySource { get; set; }
        public MethodInfo ProviderMethod { get; set; }
        public ConstructorInfo Constructor { get; set; }
        public object ReferenceObject { get; set; }
        public Type Type { get; set; }
        public bool IsSingleton { get; set; }
        public MethodInfo[] InjectableMethods { get; set; }
        public FieldInfo[] InjectableFields { get; set; }
     }
}
