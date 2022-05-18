using System;
namespace Syrup.Framework.Exceptions {
    public class DuplicateSyrupComponentException : Exception {
        public DuplicateSyrupComponentException() {
        }
        public DuplicateSyrupComponentException(string message) : base(message) {
        }
        public DuplicateSyrupComponentException(string message, Exception inner) : base(message, inner) {
        }
    }
}
