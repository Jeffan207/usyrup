using System.Reflection;
using UnityEngine;

namespace Syrup.Framework.Model {

    internal class InjectableMonoBehaviour {

        //The MB to be injected with the associated method. 
        public readonly MonoBehaviour mb;

        //The methods that will be used for method injection on the mb
        public readonly MethodInfo[] methods;

        public InjectableMonoBehaviour(MonoBehaviour mb, MethodInfo[] methods) {
            this.mb = mb;
            this.methods = methods;
        }
    }
}

