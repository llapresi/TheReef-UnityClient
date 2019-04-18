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

    public ReefWeight[] reefWeightTransitions;

    //Class to keep track of whether or not weight transitions happened yet
    public class ReefWeight
    {
        public bool shouldTransition;
        public float startingPoint;
        public float endingPoint;
        public float transitionPoint;


        public ReefWeight(float p_startingPoint, float p_endingPoint, float p_transitionPoint)
        {
            shouldTransition = true;
            startingPoint = p_startingPoint;
            endingPoint = p_endingPoint;
            transitionPoint = p_transitionPoint;
        }

        public void IsDone()
        {
            shouldTransition = false;
        }

        public bool CheckTransitionTime(float percentage){
            //We've reached the threshold, so do the transition
            //Debug.Log("Percentage: " + percentage + "TransitionPoint: " + transitionPoint + "ShouldTransition: " + shouldTransition);
            if (percentage > transitionPoint && shouldTransition)
            {
                Debug.Log("Start the transition");
                IsDone();
            }
            //Return false, until the transition is done
            return (!shouldTransition);
        }
    }//End reef weight class

    // Use this for initialization
    void Start () {

        itemManagerScript = itemManager.GetComponent<ItemSpawning>();
        trashToBeMade = itemManagerScript.trashToSpawn;

        postProcessorScript = postProcessor.GetComponent<PostProcessVolume>();
        timeManagerScript = timeObject.GetComponent<TimeManager>();

        reefWeightTransitions = new ReefWeight[10];
        PopulateReefWeight();
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

        bool atLeastTenPercent = false;

        foreach (ReefWeight r in reefWeightTransitions)
        {
            if (r.CheckTransitionTime(percentageCollected))
            {
                StartCoroutine(DecrementWeightValue(r.startingPoint, r.endingPoint));
                atLeastTenPercent = true;
            }

        }

        //Default weight if none of these conditions have been met
        if (!atLeastTenPercent)
        {
            postProcessorScript.weight = 1.0f;
        }

    }//end manage percent collected
    
    //Decrement from the start to the end point over time
    //Gradually change the post processing weight
    IEnumerator DecrementWeightValue(float start, float end)
    {
        for (float f = start; f > end; f -= 0.05f)
        {
            postProcessorScript.weight = f;
            yield return null;
        }
    }

    //Each transition decreases the post processing weight by 10%, so when we adjust the weight
    //in our coroutine, we do it in incriments. These classes will keep track of whether or
    //not the transition needs to happen, and in the future we can also trigger events with our coral
    void PopulateReefWeight()
    {
        float val = 1.0f;

        for (int i = 0; i < reefWeightTransitions.Length; i++)
        {
            reefWeightTransitions[i] = new ReefWeight(val, (val - .1f), ((i + 1) * 0.1f));
            if ( i == reefWeightTransitions.Length -1)
            {
                reefWeightTransitions[i].endingPoint = 0.0f;
            }
            val -= .1f;
            Debug.Log("Index: " + i + "Start: " + reefWeightTransitions[i].startingPoint + "End: " + reefWeightTransitions[i].endingPoint + "Start: " + reefWeightTransitions[i].transitionPoint );
        }

    }//End PopulateReefWeight

    public float GetPercentCollectedRounded()
    {
        int value = Mathf.RoundToInt(percentageCollected * 100);
        return value;
    }
    
}
