using Syrup.Framework.Declarative;

namespace Syrup.Framework {
    public interface ISyrupModule {

        void Configure(IBinder binder) { }
    }
}
