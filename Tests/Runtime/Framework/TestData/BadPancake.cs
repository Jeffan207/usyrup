using Syrup.Framework.Attributes;
using Syrup.Framework.Containers;
using Tests.Framework.TestData;
using UnityEngine;

//There is actually no such thing as a bad pancake, but somebody has to be the bad guy for our example
public class BadPancake : MonoBehaviour {
    // BadPancakes also require lazy flour, who would have guessed!
    [Inject]
    public LazyObject<Flour> lazyFlour;

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
