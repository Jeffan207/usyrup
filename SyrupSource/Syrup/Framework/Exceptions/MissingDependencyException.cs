using System;
namespace Syrup.Framework.Exceptions {
    public class MissingDependencyException : Exception {
        public MissingDependencyException() {
        }

        public MissingDependencyException(string message)
            : base(message) {
        }

        public MissingDependencyException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
