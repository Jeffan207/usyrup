using System;
using System.Collections.Generic;

namespace Syrup.Framework.Model {
    internal class NamedDependency {
        public readonly string name;
        public readonly Type type;

        public NamedDependency(string name, Type type) {
            this.name = name;
            this.type = type;
        }

        public override bool Equals(object obj) =>
            obj is NamedDependency dependency &&
            name == dependency.name &&
            EqualityComparer<Type>.Default.Equals(type, dependency.type);

        public override int GetHashCode() => HashCode.Combine(name, type);

        public override string ToString() =>
            name != null ? $"Named(\"{name}\")[{type}]" : $"[{type}]";
    }
}
