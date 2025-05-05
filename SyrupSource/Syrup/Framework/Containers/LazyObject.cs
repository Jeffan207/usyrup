namespace Syrup.Framework.Containers {

    /// <summary>
    /// A lazy container is a wrapper over the dependency that lets you delay
    /// it's instantiation until you need it in your application.
    /// Note: Lazy<> is already provided by System, so we use the name LazyObject instead
    /// to work around annoying class naming collisions.
    /// </summary>
    public class LazyObject<T> {

        internal string name;
        internal T containedType;
        internal SyrupInjector syrupInjector;

        /// <summary>
        /// You may use this constructor as a shortcut to using on-demand injection
        /// for lazily injected objects. Just note that if the object you intend to
        /// inject is a singleton then any lazy container you create for the object
        /// outside of the DI framework (i.e. by hand/manually, with this method)
        /// obviously won't be a singleton since it's out of the scope of USryup's knowledge.
        /// In those cases it's recommended to use fetch the LazyObject directly from the
        /// SyrupInjector.
        ///
        /// Examples:
        /// Option 1 - LazyObject Constructor Shortcut (Not Recommended for Singleton Dependencies):
        /// <pre>
        /// LazyObject<TastySyrup> lazySyrup = new LazyObject<TastySyrup>();
        /// </pre>
        ///
        /// Option 2 - Use SyrupInjector to get LazyObject (Recommended in General):
        /// <pre>
        /// LazyObject<TastySyrup> lazySyrup = SyrupInjector.GetInstance<LazyObject<TastySyrup>>();
        /// </pre>
        ///
        /// Caveats: You cannot provide LazyObject's directly in SyrupModules. The SyrupInjector
        /// does a lot of black magic under the hood to parse LazyObject types and build the
        /// necessary wrapped dependency. Just provide a regular version of the dependency
        /// and in the injectable class request a LazyObject version of it. USyrup will
        /// do the rest of the work to provide it.
        ///
        /// </summary>
        public LazyObject() {
            this.name = null;
        }

        public LazyObject(string name) {
            this.name = name;
        }

        /// <summary>
        /// For internal framework use only. Do not use. This needs to be public
        /// for the reflection that's being used to work.
        /// (The ultimate irony, we can't use DI for the injector itself!)
        /// </summary>
        public LazyObject(string name, SyrupInjector syrupInjector) {
            this.name = name;
            this.syrupInjector = syrupInjector;
        }

        public T Get() {
            if (containedType == null) {
                if (syrupInjector != null) {
                    containedType = syrupInjector.GetInstance<T>(name);
                } else {
                    containedType = SyrupComponent.SyrupInjector.GetInstance<T>(name);
                }

            }
            return containedType;
        }
    }
}


