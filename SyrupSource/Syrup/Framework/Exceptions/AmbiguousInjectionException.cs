using System;
namespace Syrup.Framework.Exceptions {
    public class AmbiguousInjectionException : Exception {
        public AmbiguousInjectionException() {
        }

        public AmbiguousInjectionException(string message)
            : base(message) {
        }

        public AmbiguousInjectionException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
