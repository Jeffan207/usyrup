using Syrup.Framework;
using Syrup.Framework.Attributes;
using UnityEngine;

public class ExampleAwakeSyrupModule : MonoBehaviour, ISyrupModule {

    [SerializeField]
    private AudioSource audioSource;
    
    [Provides]
    public AudioSource ProvidesAudioSource() {
        return audioSource;
    }
}
