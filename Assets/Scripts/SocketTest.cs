using UnityEngine;
using UnityEngine.Events;
using System;
using WebSocketSharp;
using System.Collections.Generic;

public class StoredPlayerFire
{
    public bool hasBeenChecked;

    public StoredPlayerFire()
    {
        hasBeenChecked = false;
    }
}

public class StoredCursorMove
{
    public Vector3 storedRotation;
    public bool hasBeenChecked;

    public StoredCursorMove(Vector3 p_storedRotation)
    {
        storedRotation = p_storedRotation;
        hasBeenChecked = false;
    }
}

[System.Serializable]
public class Vector3Event : UnityEvent<Vector3>
{
}

[System.Serializable]
public class WebSocketParamEvent: UnityEvent<WebSocket>
{
}

public class SocketTest : MonoBehaviour {

    public string serverURL;
    WebSocket webSocket;
    public Camera camera;

    List<StoredCursorMove> queuedCursorMoves;
    List<StoredPlayerFire> queuedPlayerFires;

    public Vector3Event cursorMoveEvent;
    public WebSocketParamEvent fireEvent;
    

    // Use this for initialization
    void Start()
    {
        // Init non socket connection related stuff
        queuedPlayerFires = new List<StoredPlayerFire>();
        queuedCursorMoves = new List<StoredCursorMove>();
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

        //debug line
        //Color color = new Color(0.0f, 0.0f, 1.0f);
        //Debug.DrawLine(Vector3.zero, aimForward * 20.0f, Color.green, 2.0f);

        foreach (StoredCursorMove move in queuedCursorMoves)
        {
            cursorMoveEvent.Invoke(move.storedRotation);
            move.hasBeenChecked = true;
        }
        queuedCursorMoves.RemoveAll(x => x.hasBeenChecked == true);

        foreach (StoredPlayerFire fire in queuedPlayerFires)
        {
            fireEvent.Invoke(webSocket);
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
                Vector3 ourVector = new Vector3(rotMsg.x, rotMsg.y, rotMsg.z);
                queuedCursorMoves.Add(new StoredCursorMove(ourVector));
                break;
            case "fire":
                queuedPlayerFires.Add(new StoredPlayerFire());
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
