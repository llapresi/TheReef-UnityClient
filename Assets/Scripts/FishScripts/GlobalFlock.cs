using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour {

    //MAIN CAMERA
    private GameObject myCam;

    //FISH
    public GameObject fishPrefab;
    //Spawn space
    public static int seaSize = 5;

    //Number of fish to spawn, preset for now
    static int numFish = 20;

    //Array of all our fishies
    public static GameObject[] fishes = new GameObject[numFish];

    //Goal position for our fish to flock toward
    public static Vector3 goalPos = Vector3.zero;

	// Use this for initialization
	void Start () {
        //get camera and its location
        myCam = GameObject.FindGameObjectWithTag("MainCamera");
        Vector3 camPos = myCam.transform.position;

        //instantiate all fish
        //at the location of the camera
		for (int i = 0; i < numFish; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-seaSize * 2 + camPos.x, seaSize * 2 + camPos.x),
                                       Random.Range(-seaSize + camPos.y, seaSize + camPos.y),
                                       Random.Range(-seaSize + camPos.z, seaSize + camPos.z));
            fishes[i] = (GameObject)Instantiate(fishPrefab, pos, Quaternion.identity);
        }


	}
	
	// Update is called once per frame
	void Update () {
		//occasionally change our goal position
        if (Random.Range(0, 1000) < 10)
        {
            goalPos = new Vector3(Random.Range(-seaSize, seaSize),
                                Random.Range(-seaSize, seaSize),
                                Random.Range(50, 60));
        }
	}

    public Vector3 GetGoalPosition()
    {
        return goalPos;
    }
}
