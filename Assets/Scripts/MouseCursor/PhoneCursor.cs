using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private string cursorColor;
    private UnityEngine.UI.Image cursorImage;
    private WebSocket webSocket;

    //+1 point text associated with each cursor
    public GameObject plusOnePrefab;
    public RectTransform plusOnePosition;
    private Text plusOneText;

    public CursorImageManager cursorImages;

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
        cursorImages = GameObject.FindGameObjectWithTag("CursorImages").GetComponent<CursorImageManager>();
        camera = Camera.main;
        GameObject newUICursor = Instantiate(cursorPrefab);
        GameObject uiCanvas = GameObject.Find("Canvas");
        //uiCanvas.GetComponent<Canvas>().worldCamera.
        newUICursor.transform.SetParent(uiCanvas.transform);
        cursorPosition = newUICursor.GetComponent<RectTransform>();
        cursorPosition.localScale = Vector3.one;
        cursorImage = newUICursor.GetComponent<Image>();
        cursorImage.sprite = cursorImages.GetNewCursorImage();
        cursorColor = cursorImage.sprite.name;
        Debug.Log("Cursor Color: " + cursorColor);
        offset = new Vector3(0.0f, 0.0f, 0.0f);
        RectTransform canvasSize = uiCanvas.GetComponent<RectTransform>();
        screenDimensions = new Vector2(canvasSize.rect.width, canvasSize.rect.height);
        SetupCursorColor(webSocket);

        GameObject plusOne = Instantiate(plusOnePrefab);
        plusOne.transform.SetParent(uiCanvas.transform);
        plusOnePosition = plusOne.GetComponent<RectTransform>(); ;
        plusOnePosition.localScale = Vector3.one;
        plusOneText = plusOne.GetComponent<Text>();
        plusOneText.CrossFadeAlpha(0.0f, 0.0f, false);
    }

    // Update is called once per frame
    void Update() {
        checkOffset();
        UpdatePlusOneUI();
    }

    public void SetWebSocket(WebSocket ws)
    {
        webSocket = ws;
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
        //cursorColor and cursorImage are null, so this doesnt work
        //cursorColor = cursorImage.sprite.name;

        PlayerColorMessage msg = new PlayerColorMessage();
        msg.userID = userID;
        msg.type = "playerColor";
        msg.hexColor = "red";//just send a color so it doesnt break
        //Debug.Log("MESSAGE HEX: " + msg.hexColor);
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
            //targetWeHit.changingColor = cursorColor;

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
        //Give back this sprite to the cursor image manager, and add it back to the list
        cursorImages.GiveBackCursorImage(cursorImage.sprite);
        Destroy(cursorPosition.gameObject);
        Destroy(plusOnePosition.gameObject);
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

    void UpdatePlusOneUI()
    {
        Vector3 giveXOffset = cursorPosition.anchoredPosition;
        giveXOffset.x -= 85.0f;
        giveXOffset.y += 85.0f;
        plusOnePosition.anchoredPosition = giveXOffset;
    }

    public void ShowPlusOneOnPoint()
    {
        plusOneText.CrossFadeAlpha(1.0f, 0.5f, false);
        StartCoroutine(ExecuteAfterTime(1.0f));
    }

    IEnumerator ExecuteAfterTime(float time)
    {
            yield return new WaitForSeconds(time);

            // Code to execute after the delay
            plusOneText.CrossFadeAlpha(0.0f, 0.5f, false);
    }
}
