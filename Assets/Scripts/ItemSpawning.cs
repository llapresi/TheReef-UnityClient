using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawning : MonoBehaviour {

    public GameObject bottlePrefab;
    public float spawnRate = 3.0f;
    private float timeSinceLastSpawn = 0.0f;

    public Transform[] spawnPoints;

    private List<GameObject> trashItems;

	// Use this for initialization
	void Start () {
        trashItems = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        timeSinceLastSpawn += Time.deltaTime;
		if (timeSinceLastSpawn >= spawnRate)
        {
            spawnTrash();
            timeSinceLastSpawn = 0.0f;
        }
        moveTrash();
	}

    void spawnTrash()
    {
        GameObject tempObj = Instantiate(bottlePrefab, new Vector3( 0.0f, 0.0f, 10.0f), Quaternion.identity);
        trashItems.Add(tempObj);
    }

    void moveTrash()
    {
        foreach (GameObject trash in trashItems)
        {
            Vector3 tempPos = trash.transform.position;
            tempPos.y -= 0.5f;
            trash.transform.position = tempPos;
        }
    }
}
