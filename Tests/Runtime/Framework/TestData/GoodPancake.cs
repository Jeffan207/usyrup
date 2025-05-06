using Syrup.Framework.Attributes;
using UnityEngine;

public class GoodPancake : MonoBehaviour {
    public PureMapleSyrup pureMapleSyrup;

    private void Awake() {
        //You can still init your injectable monobehaviours using awake/start
        Debug.Log("Serving up some pancakes for breakfast!");
    }

    [Inject]
    public void Init(PureMapleSyrup pureMapleSyrup) {
        this.pureMapleSyrup = pureMapleSyrup;
        pureMapleSyrup.Pour();
    }
}
