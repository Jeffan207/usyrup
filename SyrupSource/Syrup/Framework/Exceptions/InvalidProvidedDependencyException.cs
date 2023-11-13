using System;
namespace Syrup.Framework.Exceptions {
    public class InvalidProvidedDependencyException : Exception {
        public InvalidProvidedDependencyException() {
        }

        public InvalidProvidedDependencyException(string message)
            : base(message) {
        }

        public InvalidProvidedDependencyException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
