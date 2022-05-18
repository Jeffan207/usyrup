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

        public override bool Equals(object obj) {
            return obj is NamedDependency type &&
                   name == type.name &&
                   EqualityComparer<Type>.Default.Equals(this.type, type.type);
        }

        public override int GetHashCode() {
            return HashCode.Combine(name, type);
        }

        public override string ToString() {
            if (name != null) {
                return string.Format("Named(\"{0}\")[{1}]", name, type.ToString());
            }
            return string.Format("[{0}]",type.ToString());            
        }
    }
}
