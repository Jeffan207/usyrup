using System;

namespace Syrup.Framework {
    public interface ISyrupModule {

        void Configure(IBinder binder) { }
    }
}
