using System;
namespace Syrup.Framework.Exceptions {
    public class UnknownDependencySourceException : Exception {
        public UnknownDependencySourceException() {
        }

        public UnknownDependencySourceException(string message)
            : base(message) {
        }

        public UnknownDependencySourceException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
