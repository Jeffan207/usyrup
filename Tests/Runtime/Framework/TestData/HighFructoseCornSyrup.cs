using System;
using UnityEngine;

/// <summary>
///     The fake stuff, not as good as the real stuff but good enough for a high school cafeteria
/// </summary>
public class HighFructoseCornSyrup {
    public readonly string id;

    public HighFructoseCornSyrup() => id = Guid.NewGuid().ToString();

    public void Pour() {
        Debug.Log(string.Format("Pouring high fructose corn syrup '{0}'!", id));
    }
}
