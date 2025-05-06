using Syrup.Framework;
using Syrup.Framework.Attributes;
using UnityEngine;

//Your module should be a MonoBehaviour so you can attach it to the SyrupComponent in your scene
public class ExampleSyrupModule : MonoBehaviour, ISyrupModule {
    [Provides]
    public PureMapleSyrup ProvidesPureMapleSyrup(MapleSap mapleSap) => new(mapleSap);

    [Provides] //Not a singleton so a new instance will be created everytime its needed
    public MapleSap ProvidesMapleSap() => new();

    //Only a single bottle of the bad stuff to go around
    [Provides]
    [Singleton]
    public HighFructoseCornSyrup ProvidesHighFructoseCornSyrup() => new();
}
