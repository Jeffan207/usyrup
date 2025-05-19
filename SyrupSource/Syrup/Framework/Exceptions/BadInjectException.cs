using System;

namespace Syrup.Framework.Exceptions {
    public class BadInjectException : Exception {
        public BadInjectException() {
        }

        public BadInjectException(string message)
            : base(message) {
        }

        public BadInjectException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}