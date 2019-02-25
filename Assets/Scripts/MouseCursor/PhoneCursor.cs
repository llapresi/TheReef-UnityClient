using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class PhoneCursor : MonoBehaviour {

    public RectTransform cursorPosition;
    public float cursorSensitivity = 20;
    public Camera camera;
    public int userID;
    public GameObject cursorPrefab;
    public TargetParent heldItem;
    public Vector3 offset;
    public Vector3 screenDimensions;
    public float edgeOffset;

    Vector3 storedRotation;

    public void SetStoredRotation(Vector3 p_rotation)
    {
        storedRotation.x = -p_rotation.x;
        storedRotation.y = -(p_rotation.z);
        storedRotation.z = -p_rotation.y;
    }

    // Use this for initialization
    void Start () {
        camera = Camera.main;
        GameObject newUICursor = Instantiate(cursorPrefab);
        GameObject uiCanvas = GameObject.Find("Canvas");
        //uiCanvas.GetComponent<Canvas>().worldCamera.
        newUICursor.transform.SetParent(uiCanvas.transform);
        cursorPosition = newUICursor.GetComponent<RectTransform>();
        cursorPosition.localScale = Vector3.one;
        UnityEngine.UI.Image cursorImage = newUICursor.GetComponent<UnityEngine.UI.Image>();
        cursorImage.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
        offset = new Vector3(0.0f, 0.0f, 0.0f);
        RectTransform canvasSize = uiCanvas.GetComponent<RectTransform>();
        screenDimensions = new Vector2(canvasSize.rect.width, canvasSize.rect.height);
        Debug.Log("Screen Dimensions: " + screenDimensions);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 aimForward = Quaternion.Euler(storedRotation) * Vector3.forward;
        cursorPosition.anchoredPosition = new Vector3(aimForward.x * cursorSensitivity, aimForward.y * cursorSensitivity, 0);
        checkOffset();
    }

    public void Fire(WebSocket webSocket)
    {
        // Actually firing the rays right here is a horrible design but this is a one day proof of concept
        // so yeah

        // Bit shift the index of the layer (9) to get a bit mask
        // Only casts on layer 9 (targets)
        int layerMask = 1 << 9;

        RaycastHit hit;
        // Does the ray intersect any objects on layer 9
        var Ray = camera.ScreenPointToRay(cursorPosition.position);
        if (Physics.Raycast(Ray, out hit, 70.0f, layerMask))
        {
            // Get the target info component
            Debug.Log("Did Hit");
            TargetParent targetWeHit = hit.collider.gameObject.GetComponent<TargetParent>();
            // Send message with target stuff
            TargetInfoMessage msg = new TargetInfoMessage();
            heldItem = targetWeHit;
            msg.targetName = targetWeHit.targetInfo.targetName;
            msg.targetDescription = targetWeHit.targetInfo.targetDescription;
            msg.type = "targetInfo";
            msg.userID = userID;
            targetWeHit.DoHit(this);
            webSocket.Send(JsonUtility.ToJson(msg));
        }
    }

    public void Release(WebSocket webSocket)
    {
        if (heldItem != null)
        {
            heldItem.DoYeet();
            heldItem.heldBy = null;
            heldItem = null;
        }

    }

    void OnDestroy()
    {
        Destroy(cursorPosition.gameObject);
    }

    void checkOffset()
    {
        Debug.Log((-screenDimensions.x) + edgeOffset);
        Debug.Log(cursorPosition.anchoredPosition.x);
        if (cursorPosition.anchoredPosition.x < (-screenDimensions.x) + edgeOffset)
        {
            Debug.Log("Left of Screen");
        }
        if (cursorPosition.anchoredPosition.y < -screenDimensions.y)
        {
            Debug.Log("Bottom of Screen");
        }
    }
}
