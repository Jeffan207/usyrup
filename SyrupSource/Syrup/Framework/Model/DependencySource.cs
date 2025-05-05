namespace Syrup.Framework.Model {
    internal enum DependencySource {
        /// <summary>
        /// This dependency is provided via a provided method. Dependencies that are provided are expected
        /// to be fully formed.
        /// </summary>
        PROVIDER,
        /// <summary>
        /// This dependency is provided via constructor injection. It may also have members that need to be
        /// injected in addition to the constructor (such as methods or fields)
        /// </summary>
        CONSTRUCTOR
    }
}
