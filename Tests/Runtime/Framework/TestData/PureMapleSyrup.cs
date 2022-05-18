using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The good good stuff, not the B grade stuff (which is still good, but this better).
/// </summary>
public class PureMapleSyrup {

    public readonly string id;
    public readonly MapleSap mapleSap;

    public PureMapleSyrup(MapleSap mapleSap) {
        id = System.Guid.NewGuid().ToString();
        this.mapleSap = mapleSap;
    }

    public void Pour() {
        Debug.Log(string.Format("Pouring maple syrup '{0}' made with maple sap '{1}'!", id, mapleSap.id));
    }
}
