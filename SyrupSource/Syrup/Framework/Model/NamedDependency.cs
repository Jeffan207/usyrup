using System;
using System.Collections.Generic;

namespace Syrup.Framework.Model {
    internal class NamedDependency {
        public readonly string Name;
        public readonly Type Type;

        public NamedDependency(string name, Type type) {
            Name = name;
            Type = type;
        }

        public override bool Equals(object obj) =>
            obj is NamedDependency dependency &&
            Name == dependency.Name &&
            EqualityComparer<Type>.Default.Equals(Type, dependency.Type);

        public override int GetHashCode() => HashCode.Combine(Name, Type);

        public override string ToString() =>
            Name != null ? $"Named(\"{Name}\")[{Type}]" : $"[{Type}]";
    }
}
