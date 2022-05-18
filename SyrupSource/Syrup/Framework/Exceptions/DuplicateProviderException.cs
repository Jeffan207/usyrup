using System;
namespace Syrup.Framework.Exceptions {
    public class DuplicateProviderException : Exception {
        public DuplicateProviderException() {
        }

        public DuplicateProviderException(string message)
            : base(message) {
        }

        public DuplicateProviderException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
