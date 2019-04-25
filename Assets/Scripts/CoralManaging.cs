using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CoralManaging : MonoBehaviour {

    public GameObject itemManager;
    private ItemSpawning itemManagerScript;
    private int trashToBeMade;
    private float percentageCollected;

    public GameObject postProcessor;
    public PostProcessVolume postProcessorScript;

    public GameObject timeObject;
    private TimeManager timeManagerScript;
    public ParticleManaging fishParticles;

    public AnimationCurve postProcessingWeightCurve;
    public AnimationCurve fishSpawnRateCurve;

    

    // Use this for initialization
    void Start () {

        itemManagerScript = itemManager.GetComponent<ItemSpawning>();
        trashToBeMade = itemManagerScript.trashToSpawn;

        postProcessorScript = postProcessor.GetComponent<PostProcessVolume>();
        timeManagerScript = timeObject.GetComponent<TimeManager>();
    }
	
	// Update is called once per frame
	void Update () {
       ManagePercentageCollected();
	}

    void ManagePercentageCollected()
    {
        //Get a percentage by constantly dividing our trash collected by the total trash to be collected
        percentageCollected = ((float)itemManagerScript.totalTrashCollected / (float)trashToBeMade);

        //Theres a more efficient way to set this up. Will wait to see how we're managing our objects before
        //We implment the class that holds the corals and weight values for post processing
        //for now, this poopoo will suffice

        //Boot us out if we havent started the timer yet. This holds off on the post processing weight adjustments during onboarding
        if (!timeManagerScript.IsTimerRunning())
        {
            return;
        }

        // Evaluate weight curve
        float currentWeightValue = postProcessingWeightCurve.Evaluate(percentageCollected);
        postProcessorScript.weight = currentWeightValue;
        AdjustFishParticles(percentageCollected);
    }

    public float GetPercentCollectedRounded()
    {
        int value = Mathf.RoundToInt(percentageCollected * 100);

        //Check just in case...
        if (value >= 100)
        {
            value = 100;
        }
        return value;
    }

    public float GetPercentCollected()
    {
        return percentageCollected;
    }

    void AdjustFishParticles(float collected)
    {
        if (collected > .3f)
        {
            fishParticles.speed = 1.0f;
            fishParticles.shouldSpawnFish = true;
            float newRate = fishSpawnRateCurve.Evaluate(collected);
            fishParticles.SetRate(newRate);
        }
        else
        {
            fishParticles.shouldSpawnFish = false;
        }
    }
}
