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

    public AnimationCurve postProcessingWeightCurve;

    //Class to keep track of whether or not weight transitions happened yet
    public class ReefWeight
    {
        public bool shouldTransition;
        public float startingPoint;
        public float endingPoint;
        public float transitionPoint;

        //Class to hold transitions for reefWeight
        //Parameters:
        //Starting float of post processing weight
        //ending float of post processing weight to transition to
        //Transtion float, which looks at the % of the reef that is clean. if > then the %, the trasition triggers
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

        public void IsNotDone()
        {
            shouldTransition = true;
        }

        public bool CheckTransitionTime(float percentage){
            //We've reached the threshold, so do the transition

            if (!shouldTransition)
            {//If it shouldnt transition, were done with it
                return false;
            }
            if (percentage > transitionPoint && shouldTransition)
            {
                IsDone();
                Debug.Log("Transitioning: " + "Start: " + startingPoint + "End: " + endingPoint + " %: " + transitionPoint);
                return true;
            }
            
            //Return false, until the transition is done
            return false;
        }
    }//End reef weight class

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
        //Debug.Log(postProcessorScript.weight);
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
}
