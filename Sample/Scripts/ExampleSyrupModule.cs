using System.Collections;
using System.Collections.Generic;
using Syrup.Framework;
using Syrup.Framework.Attributes;
using UnityEngine;

//Your module should be a MonoBehaviour so you can attach it to the SyrupComponent in your scene
public class ExampleSyrupModule : MonoBehaviour, ISyrupModule {

    [Provides]
    public PureMapleSyrup ProvidesPureMapleSyrup(MapleSap mapleSap) {
        return new PureMapleSyrup(mapleSap);
    }

    [Provides] //Not a singleton so a new instance will be created everytime its needed
    public MapleSap ProvidesMapleSap() {
        return new MapleSap();
    }


    //Only a single bottle of the bad stuff to go around
    [Provides]
    [Singleton]
    public HighFructoseCornSyrup ProvidesHighFructoseCornSyrup() {
        return new HighFructoseCornSyrup();
    }


}
