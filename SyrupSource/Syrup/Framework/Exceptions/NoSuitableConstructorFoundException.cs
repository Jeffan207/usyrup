using System;

namespace Syrup.Framework.Exceptions {
public class NoSuitableConstructorFoundException : Exception {
    public NoSuitableConstructorFoundException() {
    }

    public NoSuitableConstructorFoundException(string message)
        : base(message) {
    }

    public NoSuitableConstructorFoundException(string message, Exception inner)
        : base(message, inner) {
    }
}
}
