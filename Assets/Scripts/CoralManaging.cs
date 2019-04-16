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

    private ReefWeight[] reefWeightTransitions;

    //Class to keep track of whether or not weight transitions happened yet
    public class ReefWeight
    {
        public bool shouldTransition;
        public float startingPoint;
        public float endingPoint;


        public ReefWeight(float p_startingPoint, float p_endingPoint)
        {
            shouldTransition = true;
            startingPoint = p_startingPoint;
            endingPoint = p_endingPoint;
        }

        public void IsDone()
        {
            shouldTransition = false;
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

        //Debug.Log("Percentage: " + percentageCollected);
        //Debug.Log("Weight: " + postProcessorScript.weight);

        if (percentageCollected >= 0.975f)
        {
            if (reefWeightTransitions[9].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.1f, .0f));
                reefWeightTransitions[9].IsDone();
            }
        }
        else if (percentageCollected > .9)
        {
            if (reefWeightTransitions[8].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.2f, .1f));
                reefWeightTransitions[8].IsDone();
            }
        }
        else if (percentageCollected > .8)
        {
            if (reefWeightTransitions[7].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.3f, .2f));
                reefWeightTransitions[7].IsDone();
            }
        }
        else if (percentageCollected > .7)
        {
            if (reefWeightTransitions[6].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.4f, .3f));
                reefWeightTransitions[6].IsDone();
            }
        }
        else if (percentageCollected > .6)
        {
            if (reefWeightTransitions[5].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.5f, .4f));
                reefWeightTransitions[5].IsDone();
            }
        }
        else if (percentageCollected > .5)
        {
            if (reefWeightTransitions[4].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.6f, .5f));
                reefWeightTransitions[4].IsDone();
            }
        }
        else if (percentageCollected > .4)
        {
            if (reefWeightTransitions[3].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.7f, .6f));
                reefWeightTransitions[3].IsDone();
            }
        }
        else if (percentageCollected > .3)
        {
            if (reefWeightTransitions[2].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.8f, .7f));
                reefWeightTransitions[2].IsDone();
            }
        }
        else if (percentageCollected > .2)
        {
            if (reefWeightTransitions[1].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(.9f, .8f));
                reefWeightTransitions[1].IsDone();
            }
        }
        else if (percentageCollected > .1)
        {
            if(reefWeightTransitions[0].shouldTransition)
            {
                StartCoroutine(DecrementWeightValue(reefWeightTransitions[0].startingPoint, reefWeightTransitions[0].endingPoint));
                reefWeightTransitions[0].IsDone();
            }
        }
        else
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
        /*
        reefWeightTransitions[0] = new ReefWeight(1.0f, 0.9f);
        reefWeightTransitions[1] = new ReefWeight(0.9f, 0.8f);
        reefWeightTransitions[2] = new ReefWeight(0.8f, 0.7f);
        reefWeightTransitions[3] = new ReefWeight(0.7f, 0.6f);
        reefWeightTransitions[4] = new ReefWeight(0.6f, 0.5f);
        reefWeightTransitions[5] = new ReefWeight(0.5f, 0.4f);
        reefWeightTransitions[6] = new ReefWeight(0.4f, 0.3f);
        reefWeightTransitions[7] = new ReefWeight(0.3f, 0.2f);
        reefWeightTransitions[8] = new ReefWeight(0.2f, 0.1f);
        reefWeightTransitions[9] = new ReefWeight(0.1f, 0.0f);
        */
        float val = 1.0f;

        for (int i = 0; i < reefWeightTransitions.Length; i++)
        {
            reefWeightTransitions[i] = new ReefWeight(val, (val - .1f));
            val -= .1f;
        }
    }//End PopulateReefWeight
    
}
