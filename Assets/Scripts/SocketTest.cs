using UnityEngine;
using System;
using WebSocketSharp;
using System.Collections.Generic;

[System.Serializable]
public class RotationMessage
{
    public string type;
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class CommonTypeMessage
{
    public string type;
}

public class StoredPlayerFire
{
    public Vector3 storedRotation;
    public bool hasBeenChecked;

    public StoredPlayerFire(Vector3 p_storedRotation)
    {
        storedRotation = p_storedRotation;
        hasBeenChecked = false;
    }
}

[System.Serializable]
public class TargetInfoMessage
{
    public string type = "targetInfo";
    public string targetName;
    public string targetDescription;
}

public class SocketTest : MonoBehaviour {

    [System.Serializable]
    public class SpawnProperties
    {
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
        public float minZ;
        public float maxZ;
        public GameObject prefabToSpawn;
    }

    public SpawnProperties spawnProperties;
    public string serverURL;
    Vector3 storedRotation;
    WebSocket webSocket;
    public UnityEngine.UI.Image cursorImage;
    RectTransform cursorPosition;
    public Camera playerCamera;
    public Transform aimTransform;

    List<StoredPlayerFire> queuedPlayerFires;

    public UnityEngine.Event fireEvent;
    

    // Use this for initialization
    void Start()
    {
        // Init non socket connection related stuff
        queuedPlayerFires = new List<StoredPlayerFire>();
        cursorPosition = cursorImage.GetComponent<RectTransform>();
        webSocket = new WebSocket(serverURL);

        // Open the socket
        Debug.Log(": Open socket: " + webSocket.ReadyState + " - Websocket Alive: " + webSocket.IsAlive);

        webSocket.OnOpen += OnOpenHandler;
        webSocket.OnMessage += OnMessageHandler;
        webSocket.OnClose += OnCloseHandler;
        webSocket.OnError += OnErrorHandler;

        if (!webSocket.IsAlive)
        {
            Debug.Log("Connecting...");
            webSocket.ConnectAsync();
        }
    }

    private void Update() {
        // Set the rotation (used to drive the cursor)
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(storedRotation));
        cursorPosition.position = playerCamera.WorldToScreenPoint(aimTransform.position);

        // Do player fire events
        foreach (StoredPlayerFire fire in queuedPlayerFires)
        {
            // Actually firing the rays right here is a horrible design but this is a one day proof of concept
            // so yeah

            // Bit shift the index of the layer (9) to get a bit mask
            // Only casts on layer 9 (targets)
            int layerMask = 1 << 9;

            RaycastHit hit;
            Debug.DrawRay(Vector3.zero, fire.storedRotation * 20, Color.yellow);
            // Does the ray intersect any objects on layer 9
            if (Physics.Raycast(Vector3.zero, fire.storedRotation, out hit, Mathf.Infinity, layerMask))
            {
                // Get the target info component
                Debug.Log("Did Hit");
                TargetParent targetWeHit = hit.collider.gameObject.GetComponent<TargetParent>();
                // Send message with target stuff
                TargetInfoMessage msg = new TargetInfoMessage();
                msg.targetName = targetWeHit.targetInfo.targetName;
                msg.targetDescription = targetWeHit.targetInfo.targetDescription;
                msg.type = "targetInfo";
                targetWeHit.DoHit();
                webSocket.Send(JsonUtility.ToJson(msg));
            }

            fire.hasBeenChecked = true;
        }
        queuedPlayerFires.RemoveAll(x => x.hasBeenChecked == true);
    }

    private void OnDestroy()
    {
        if (webSocket != null)
        {
            webSocket.Close();
        }
    }

    private void OnOpenHandler(object sender, EventArgs e)
    {
        // Log our connection to WS server
        Debug.Log("OnOpenHandler -> Open socket: " + webSocket.ReadyState + " - Websocket Alive: " + webSocket.IsAlive);

        // send message to server stating we are the unity client
        CommonTypeMessage isUnity = new CommonTypeMessage();
        isUnity.type = "isUnity";
        webSocket.Send(JsonUtility.ToJson(isUnity));
    }

    private void OnMessageHandler(object sender, MessageEventArgs e)
    {
        CommonTypeMessage msgType = JsonUtility.FromJson<CommonTypeMessage>(e.Data);

        switch(msgType.type)
        {
            case "rotate":
                RotationMessage rotMsg = JsonUtility.FromJson<RotationMessage>(e.Data);
                storedRotation.x = -rotMsg.x;
                storedRotation.y = -(rotMsg.z + 90 + 180);
                storedRotation.z = -rotMsg.y;
                break;
            case "fire":
                Quaternion quat = Quaternion.Euler(storedRotation);
                queuedPlayerFires.Add(new StoredPlayerFire(quat * Vector3.forward));
                Debug.Log("do fire stuff");
                break;
        }
    }

    private void OnCloseHandler(object sender, CloseEventArgs e)
    {
        Debug.Log("OnCloseHandler | code: " + e.Code + " reason: " + e.Reason);
    }

    private void OnSendComplete(bool success)
    {
        Debug.Log("OnSendComplete | Message sent successfully? " + success);
    }

    private void OnErrorHandler(object sender, ErrorEventArgs e)
    {
        Debug.Log("OnErrorHandler | message: " + e.Message + " - exception: " + e.Exception);
    }
}
