using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralManaging : MonoBehaviour {

    public GameObject itemManager;
    private ItemSpawning itemManagerScript;
    private int trashToBeMade;
    private float percentageCollected;

	// Use this for initialization
	void Start () {
        itemManagerScript = itemManager.GetComponent<ItemSpawning>();
        trashToBeMade = itemManagerScript.trashToSpawn;
	}
	
	// Update is called once per frame
	void Update () {
        ManagePercentageCollected();
	}

    void ManagePercentageCollected()
    {
        //Get a percentage by constantly dividing our trash collected by the total trash to be collected
        percentageCollected = ((float)itemManagerScript.totalTrashCollected / (float)trashToBeMade);
    }
}
