using UnityEngine;
using System.Collections;
using Syrup.Framework.Attributes;

//There is actually no such thing as a bad pancake, but somebody has to be the bad guy for our example
public class BadPancake : MonoBehaviour {

    public HighFructoseCornSyrup highFructoseCornSyrup;

    private void Awake() {
        Debug.Log("Serving up some bad pancakes for breakfast!");
    }

    [Inject]
    public void Init(HighFructoseCornSyrup highFructoseCornSyrup) {
        this.highFructoseCornSyrup = highFructoseCornSyrup;
        highFructoseCornSyrup.Pour();
    }
}
