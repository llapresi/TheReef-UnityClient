using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManaging : MonoBehaviour {

    public ParticleSystem[] fishParticles;
    public GameObject leftFishGroup;

    public bool shouldLoop = true;

	// Use this for initialization
	void Start () {
        fishParticles = leftFishGroup.GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        //ChangeLoop();
	}

    void ChangeLoop()
    {
        //This does turn off the looping on all fish particles, but when turned back on, they dont respawn.
        for (int i = 0; i < fishParticles.Length; i++)
        {
            var main = fishParticles[i].main;
            main.loop = shouldLoop;
            //Debug.Log(fishParticles[i].main.loop);
        }
    }
}
