using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectScripts : MonoBehaviour {

    public AudioClip[] audio;
    public AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	

    public void PlayRandomBubble()
    {
        int randomInt = Random.Range(0, audio.Length);
        audioSource.PlayOneShot(audio[randomInt], 1.0f);
    }
}
