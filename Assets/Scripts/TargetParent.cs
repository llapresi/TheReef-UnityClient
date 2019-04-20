using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetParent : MonoBehaviour {

    // Use this for initialization
    public TargetInfo targetInfo;
    public Collider Collider;
    public Renderer ourRenderer;
    public PhoneCursor heldBy;
    public bool isYeeting;
    float yeetProgress;
    //Negative z: come toward you. y: go up
    public float yeetSpeed;

    [Range(0.1f, 1.0f)]
    public float yeetMidpointPosition;
    public float yeetMidpointHeight;
    public bool debugYeet = false;

    // Array of our 3 yeet positions
    Vector3[] yeetPoints;

    //Get our itemSpawning Manager
    //so we can access its properties
    private GameObject itemManager;
    private ItemSpawning itemSpawnScript;
    private float fallSpeed;


	void Start () {
        yeetProgress = 0.0f;
        yeetPoints = new Vector3[3];

        itemManager = GameObject.FindGameObjectWithTag("ItemManager");
        itemSpawnScript = itemManager.GetComponent<ItemSpawning>();
        fallSpeed = itemSpawnScript.fallSpeed;
    }
	
	// Update is called once per frame
	void Update () {
		if(heldBy != null)
        {
            Vector3 cursorPosAtOurZ = new Vector3(heldBy.cursorPosition.transform.position.x, heldBy.cursorPosition.transform.position.y, 80.0f);
            Vector3 newPosition = heldBy.camera.ScreenToWorldPoint(cursorPosAtOurZ);
            this.transform.position = newPosition;
        }

        if (isYeeting)
        {
            yeetProgress += Time.deltaTime * yeetSpeed;
            Vector3 yeetStartMidPoint = Vector3.Lerp(yeetPoints[0], yeetPoints[1], yeetProgress);
            Vector3 yeetMidEndPoint = Vector3.Lerp(yeetPoints[1], yeetPoints[2], yeetProgress);
            Vector3 newYeetPosition = Vector3.Lerp(yeetStartMidPoint, yeetMidEndPoint, yeetProgress);
            transform.position = newYeetPosition;
            if (!debugYeet)
            {
                DestroyIfYeetComplete();
            }
            else if (yeetProgress >= 1.0f)
            {
                yeetProgress = 0;
                MakeNewYeetMidPoint();
                MakeNewYeetEndpoint();
            }
        }

        if (heldBy == null)
        {
            MoveDown();
        }
	}

    public void DoHit(PhoneCursor p_userID)
    {
        //only register hits if its not already held.
        //i.e., no more thieving
        //if (heldBy == null)
        //{
            heldBy = p_userID;
        //}
        //StartCoroutine(ChangeColor());
    }

    public void DoYeet()
    {
        isYeeting = true;
        // Set our yeet start point
        yeetPoints[0] = this.transform.position;
        // Get end point of our yeet from the Scene
        MakeNewYeetEndpoint();
        // Make our yeet middle node
        MakeNewYeetMidPoint();
    }

    public void MakeNewYeetMidPoint()
    {
        yeetPoints[1] = Vector3.Lerp(yeetPoints[0], yeetPoints[2], yeetMidpointPosition);
        yeetPoints[1].y = yeetMidpointHeight;
    }
    public void MakeNewYeetEndpoint()
    {
        yeetPoints[2] = GameObject.Find("YeetEndpoint").transform.position;
    }

    public void DestroyIfYeetComplete()
    {
        if (yeetProgress >= 1.0f)
        {
            DestroyItem();
            //Since the player yeeted an object, increase the trash collection count
            itemSpawnScript.totalTrashCollected++;
        }
    }

    //Move Object down, if too far down -> Destroy
    public void MoveDown()
    {
        Vector3 tempPos = transform.position;
        tempPos.y -= (fallSpeed / .016f) * Time.deltaTime;
        transform.position = tempPos;

        if (tempPos.y <= -50.0f)
        {
            DestroyItem();
        }
    }

    public void DestroyItem()
    {
        /*
        if (itemSpawnScript.trashItems.Contains(gameObject))
        {
            itemSpawnScript.trashItems.Remove(gameObject);
        }
        */
        Destroy(this.gameObject);
    }

    IEnumerator ChangeColor()
    {
        Color originalColor = ourRenderer.material.GetColor("_Color");
        ourRenderer.material.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.6f);
        ourRenderer.material.SetColor("_Color", originalColor);
    }
}
