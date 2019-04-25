﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParticleManaging : MonoBehaviour {

    public ParticleSystem[] fishParticles;
    public GameObject leftFishGroup;
    public GameObject rightFishGroup;

    public bool shouldSpawnFish = true;
    public float speed = 1.0f;

	// Use this for initialization
	void Start () {
        ParticleSystem[] tempArray1 = leftFishGroup.GetComponentsInChildren<ParticleSystem>();
        ParticleSystem[] tempArray2 = rightFishGroup.GetComponentsInChildren<ParticleSystem>();
        fishParticles = tempArray1.Concat(tempArray2).ToArray();
	}
	
	// Update is called once per frame
	void Update () {
        ChangeLoop();
        AdjustSpeed(speed);
	}

    void ChangeLoop()
    {
        if (!shouldSpawnFish )
        {
            StopParticles();
        } else if (shouldSpawnFish)
        {
            StartParticles();
        }
    }

    public void AdjustSpeed(float speedArg)
    {
        //This does turn off the looping on all fish particles, but when turned back on, they dont respawn.
        for (int i = 0; i < fishParticles.Length; i++)
        {
            var main = fishParticles[i].main;
            main.simulationSpeed = speedArg;
            //Debug.Log(fishParticles[i].main.loop);
        }
    }

    public void StartParticles()
    {
        for (int i = 0; i < fishParticles.Length; i++)
        {
            var emission = fishParticles[i];

            if (!emission.isPlaying)
            {
                emission.Play();
            }
        }
    }

    public void StopParticles()
    {
        for (int i = 0; i < fishParticles.Length; i++)
        {
            var emission = fishParticles[i];

            if (!emission.isStopped)
            {
                emission.Stop();
            }
        }
    }

    public void SetRate(float rate)
    {
        for (int i = 0; i < fishParticles.Length; i++)
        {
            var emission = fishParticles[i].emission;

            emission.rateOverTime = rate;
        }
    }


}
