using System.Collections;
using System.Collections.Generic;
using Syrup.Framework.Attributes;
using UnityEngine;

public class MusicalMilk : MonoBehaviour  {
    
    [Inject]
    AudioSource audioSource;

    private void Start() {
        Debug.Log($"AudioSource was injected! {audioSource != null}");
    }

    public bool HasAudioSource() {
        return audioSource != null;
    }
}
