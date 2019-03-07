using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawning : MonoBehaviour {

    public GameObject bottlePrefab;
    //In the future we'll pick random prefabs of trash when we have more
    //public GameObject[] trashPrefabs;

    public float spawnRate = 3.0f;
    public float fallSpeed = 0.5f;
    private float timeSinceLastSpawn = 0.0f;

    public int totalTrashMade = 0;
    public int trashToSpawn = 100;
    public int totalTrashCollected = 0;

    //Dimensions of screen space
    float fullWidth;
    float fullHeight;

    float yRandomRange;
    float xRandomRange;

    public Transform[] spawnPoints;

    public List<GameObject> trashItems;

	// Use this for initialization
	void Start () {
        trashItems = new List<GameObject>();
        //InitializeSpawnPoints();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateTiming();
	}

    void SpawnTrash()
    {
        if (totalTrashMade < trashToSpawn)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {

                GameObject tempObj = Instantiate(bottlePrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, spawnPoints[i]);
                //Get Parents location
                Vector3 randomPosition = spawnPoints[i].position;

                xRandomRange = 15.0f;
                yRandomRange = 50.0f;

                //Stagger the locations slightly, ideally this would be done with 1/16 of the scene width
                randomPosition.x += Random.Range((-xRandomRange), xRandomRange);
                randomPosition.y += Random.value * yRandomRange;

                //Set that to our bottles location
                tempObj.transform.position = randomPosition;

                //Also give it a slight rotation
                Vector3 tweakedRotation = tempObj.transform.eulerAngles;
                tweakedRotation.z = (float)(Random.Range(0, 45) - 22.5);
                tempObj.transform.eulerAngles = tweakedRotation;

                //Add it to our list
                //wait do we even need the list
                //trashItems.Add(tempObj);

                //Incriment our trash made by 1 for each item made
                //Whenever a trash is destroyed, we'll add it to totalTrashCollected
                //at the end of the game we'll use the two to determine the % of the reef
                totalTrashMade++;
            }//end for
        }// end if      
    }

    //Some fancy positioning for our quadrants on start, but canvas is offset from the camera so the positions
    //dont exactly line up the way they should. will have to tweak. until then.... hardcode positions in scene
    //...just realized itd be easier to not have the spawnPoints centered. too late for that
    void InitializeSpawnPoints()
    {
        //Get the Size of the space we're working with
        GameObject uiCanvas = GameObject.Find("Canvas");
        RectTransform canvasSize = uiCanvas.GetComponent<RectTransform>();

        fullWidth = canvasSize.rect.width;
        fullHeight = canvasSize.rect.height;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            //Want our Spawn Points separated into quarters
            //1st, 3rd, 5th, and 7th eigth to center them
            //Have to handle first case differently
            float signChange = -1;
            if (i == 0)
            {
                signChange = 1;
            }

            float xPos = (fullWidth / 8) * ((i * 2) + (1 * signChange));

            spawnPoints[i].position = new Vector3(xPos, fullHeight/2, this.transform.position.z);
        }
    }//End Initialize Spawn Points
    void UpdateTiming()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnRate)
        {
            SpawnTrash();
            timeSinceLastSpawn = 0.0f;
        }
    }
}
