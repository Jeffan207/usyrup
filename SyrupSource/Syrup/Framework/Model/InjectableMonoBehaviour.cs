using System.Reflection;
using UnityEngine;

namespace Syrup.Framework.Model {

    internal class InjectableMonoBehaviour {

        //The MB to be injected with the associated method. 
        public readonly MonoBehaviour mb;

        //The methods that will be used for method injection on the mb
        public readonly MethodInfo[] methods;

        //The fields that will be used for field injection on the mb
        public readonly FieldInfo[] fields;

        public InjectableMonoBehaviour(MonoBehaviour mb, MethodInfo[] methods, FieldInfo[] fields) {
            this.mb = mb;
            this.methods = methods;
            this.fields = fields;
        }
    }
}

