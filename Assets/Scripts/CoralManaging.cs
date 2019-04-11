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
    private PostProcessVolume postProcessorScript;

	// Use this for initialization
	void Start () {
        itemManagerScript = itemManager.GetComponent<ItemSpawning>();
        trashToBeMade = itemManagerScript.trashToSpawn;

        postProcessorScript = postProcessor.GetComponent<PostProcessVolume>();
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

        if (percentageCollected > .2)
        {
            StartCoroutine(DecrementWeightValue(.6f, .4f));
        }
        else if (percentageCollected > .1)
        {
            StartCoroutine(DecrementWeightValue(.8f, .6f));
        } else
        {
            postProcessorScript.weight = .8f;
        }
    }
    
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
    
}
