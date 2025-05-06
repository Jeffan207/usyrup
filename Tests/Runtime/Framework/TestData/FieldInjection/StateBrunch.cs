using System.Collections;
using System.Collections.Generic;
using Syrup.Framework.Attributes;
using UnityEngine;

namespace Tests.Framework.TestData {
    public class StateBrunch : Identifiable {

        [Inject]
        public Egg egg;

        public Butter butter;

        [Inject]
        public StateBrunch() : base() { }

        [Inject]
        public void InitStateBrunch(Butter butter) {
            this.butter = butter;
        }

    }
}
