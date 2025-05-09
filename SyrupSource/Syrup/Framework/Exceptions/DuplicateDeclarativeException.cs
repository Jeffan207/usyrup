using System;
namespace Syrup.Framework.Exceptions {
    public class DuplicateDeclarativeException : Exception {
        public DuplicateDeclarativeException() {
        }

        public DuplicateDeclarativeException(string message)
            : base(message) {
        }

        public DuplicateDeclarativeException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
