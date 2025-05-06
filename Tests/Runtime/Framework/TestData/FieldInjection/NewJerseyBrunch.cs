using System;
using Syrup.Framework.Attributes;

namespace Tests.Framework.TestData {
    public class NewJerseyBrunch : StateBrunch {
        [Inject]
        public OrangeJuice orangeJuice;

        public Pancake pancake;

        [Inject]
        public NewJerseyBrunch() { }

        [Inject]
        public void InitNewJerseyBrunch(Pancake pancake) {
            if (butter == null) {
                throw new ArgumentException("Can't have a pancake brunch without butter!");
            }

            this.pancake = pancake;
        }
    }
}
