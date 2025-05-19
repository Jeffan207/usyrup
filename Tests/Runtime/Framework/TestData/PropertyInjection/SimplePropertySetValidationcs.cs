using System;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData.PropertyInjection {
    public class SimplePropertySetValidation {

        private int propNum;
        
        [Inject]
        public int PropNum {
            get => propNum;
            set {
                if (value > 100) {
                    throw new ArgumentException("PropNum > 100");
                }
                propNum = value;
            } 
        }
    }
}