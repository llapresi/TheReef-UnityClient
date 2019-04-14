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
    private Color cursorColor;

    // Storing our current and previous rotations for cursor interp
    Vector2 storedRotation;
    Vector2 prevStoredRotation;
    float currentCursorTime;
    float prevCursorTime;
    public bool useInterpolation = true;

    public void SetStoredRotation(Vector2 p_rotation)
    {
        // reset our prev state
        prevStoredRotation = storedRotation;
        prevCursorTime = currentCursorTime;
        // set our new rotation
        storedRotation.x = -p_rotation.x;
        storedRotation.y = -(p_rotation.y);
        currentCursorTime = Time.time;
    }

    // Use this for initialization
    void Start() {
        camera = Camera.main;
        GameObject newUICursor = Instantiate(cursorPrefab);
        GameObject uiCanvas = GameObject.Find("Canvas");
        //uiCanvas.GetComponent<Canvas>().worldCamera.
        newUICursor.transform.SetParent(uiCanvas.transform);
        cursorPosition = newUICursor.GetComponent<RectTransform>();
        cursorPosition.localScale = Vector3.one;
        UnityEngine.UI.Image cursorImage = newUICursor.GetComponent<UnityEngine.UI.Image>();
        cursorImage.color = cursorColor;
        offset = new Vector3(0.0f, 0.0f, 0.0f);
        RectTransform canvasSize = uiCanvas.GetComponent<RectTransform>();
        screenDimensions = new Vector2(canvasSize.rect.width, canvasSize.rect.height);
    }

    // Update is called once per frame
    void Update() {
        checkOffset();
    }

    public void Fire(WebSocket webSocket)
    {
        //They shouldnt be able to pick up multiple trash items at once
        if (heldItem != null)
        {
            return;
        }

        // Bit shift the index of the layer (9) to get a bit mask
        // Only casts on layer 9 (targets)
        int layerMask = 1 << 9;

        RaycastHit hit;
        // Does the ray intersect any objects on layer 9
        var Ray = camera.ScreenPointToRay(cursorPosition.position);
        if (Physics.Raycast(Ray, out hit, 300.0f, layerMask))
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

    public void SetupCursorColor(WebSocket webSocket)
    {
        cursorColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);

        PlayerColorMessage msg = new PlayerColorMessage();
        msg.userID = userID;
        msg.type = "playerColor";
        msg.hexColor = ColorUtility.ToHtmlStringRGB(cursorColor);
        webSocket.Send(JsonUtility.ToJson(msg));
    }

    public void ConstantFire(WebSocket webSocket)
    {
        // Bit shift the index of the layer (9) to get a bit mask
        // Only casts on layer 9 (targets)
        int layerMask = 1 << 9;

        RaycastHit hit;
        // Does the ray intersect any objects on layer 9
        var Ray = camera.ScreenPointToRay(cursorPosition.position);
        if (Physics.Raycast(Ray, out hit, 100.0f, layerMask))
        {
            // Get the target info component
            Debug.Log("Did Hit");

            //Starting GameObject now
            StartGame targetWeHit = hit.collider.gameObject.GetComponent<StartGame>();
            //Let the target know we hit it
            targetWeHit.isHovered = true;
            targetWeHit.changingColor = cursorColor;

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
        //Get distance to our edges
        float halfWidth = (screenDimensions.x) / 2;
        float halfHeight = (screenDimensions.y) / 2;

        //Normal Movement w/ interp
        Vector3 aimForward = Quaternion.Euler(storedRotation) * Vector3.forward;
        Vector3 prevAimFoward = Quaternion.Euler(prevStoredRotation) * Vector3.forward;

        Vector3 lerpAim = Vector3.LerpUnclamped(prevAimFoward, aimForward, CalculateInterpFactor());

        // Use interpolated aim if bool is set to true, otherwise just use our current aim
        if(useInterpolation)
        {
            cursorPosition.anchoredPosition = new Vector3(lerpAim.x * cursorSensitivity, lerpAim.y * cursorSensitivity, 0);
        }
        else
        {
            cursorPosition.anchoredPosition = new Vector3(aimForward.x * cursorSensitivity, aimForward.y * cursorSensitivity, 0);
        }

        //Our cursors XY coords
        float tempX = cursorPosition.anchoredPosition.x;
        float tempY = cursorPosition.anchoredPosition.y;

        //If they hit any edges, lock the position to that edge
        if (cursorPosition.anchoredPosition.x < (-halfWidth))
        {
            //Left Edge
            cursorPosition.anchoredPosition = new Vector3(-halfWidth, tempY, 0);
        }
        if (cursorPosition.anchoredPosition.y < (-halfHeight))
        {
            //Bottom Edge
            cursorPosition.anchoredPosition = new Vector3(tempX, (-halfHeight), 0);
        }
        if (cursorPosition.anchoredPosition.x > (halfWidth))
        {
            //Right Edge
            cursorPosition.anchoredPosition = new Vector3(halfWidth, tempY, 0);
        }
        if (cursorPosition.anchoredPosition.y > (halfHeight))
        {
            //Top Edge
            cursorPosition.anchoredPosition = new Vector3(tempX, (halfHeight), 0);
        }

    }

    float CalculateInterpFactor()
    {
        float newerTime = currentCursorTime;
        float olderTime = prevCursorTime;

        if (newerTime != olderTime)
        {
            return (Time.time - newerTime) / (newerTime - olderTime);
        }
        else
        {
            return 1;
        }
    }
}
