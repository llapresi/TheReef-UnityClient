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
    //Negative z: come toward you. y: go up
    public Vector2 yeetSpeed = new Vector2(-20.0f, 50.0f);


	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		if(heldBy != null)
        {
            Vector3 cursorPosAtOurZ = new Vector3(heldBy.cursorPosition.transform.position.x, heldBy.cursorPosition.transform.position.y, 50.0f);
            Vector3 newPosition = heldBy.camera.ScreenToWorldPoint(cursorPosAtOurZ);
            this.transform.position = newPosition;
        }

        if (isYeeting)
        {
            Vector2 deltaYeet = new Vector2(yeetSpeed.x * Time.deltaTime, yeetSpeed.y * Time.deltaTime);
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + deltaYeet.y, this.transform.position.z + deltaYeet.x);
        }
	}

    public void DoHit(PhoneCursor p_userID)
    {
        heldBy = p_userID;
        StartCoroutine(ChangeColor());
    }

    public void DoYeet()
    {
        isYeeting = true;
    }

    IEnumerator ChangeColor()
    {
        Color originalColor = ourRenderer.material.GetColor("_Color");
        ourRenderer.material.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.6f);
        ourRenderer.material.SetColor("_Color", originalColor);
    }
}
