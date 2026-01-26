using Syrup.Framework;
using Syrup.Framework.Attributes;
using UnityEngine;

public class ExampleAwakeSyrupModule : SyrupModule {
    
    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }
    
    [Provides]
    public AudioSource ProvidesAudioSource() {
        return audioSource;
    }
}
