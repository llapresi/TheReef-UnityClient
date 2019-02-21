using UnityEngine;
using UnityEngine.Events;
using System;
using WebSocketSharp;
using System.Collections.Generic;
using System.Linq;

public class StoredPlayerFire
{
    public bool hasBeenChecked;
    public int userID;

    public StoredPlayerFire(int p_id)
    {
        hasBeenChecked = false;
        userID = p_id;
    }
}

public class StoredCursorMove
{
    public Vector3 storedRotation;
    public bool hasBeenChecked;
    public int userID;

    public StoredCursorMove(Vector3 p_storedRotation, int p_id)
    {
        storedRotation = p_storedRotation;
        hasBeenChecked = false;
        userID = p_id;
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
    public List<PhoneCursor> users;
    public GameObject phoneCursorPrefab;

    List<StoredCursorMove> queuedCursorMoves;
    List<StoredPlayerFire> queuedPlayerFires;
    List<int> queuedUsers;
    List<int> queuedDisconnects;

    public Vector3Event cursorMoveEvent;
    public WebSocketParamEvent fireEvent;
    

    // Use this for initialization
    void Start()
    {
        // Init non socket connection related stuff
        queuedPlayerFires = new List<StoredPlayerFire>();
        queuedCursorMoves = new List<StoredCursorMove>();
        queuedUsers = new List<int>();
        queuedDisconnects = new List<int>();
        users = new List<PhoneCursor>();
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

        //Make queuedCursorMoves a new list, so changing it doesnt throw err
        foreach (StoredCursorMove move in queuedCursorMoves)
        {
            foreach (PhoneCursor user in users)
            {
                if (user.userID == move.userID)
                {
                    user.SetStoredRotation(move.storedRotation);
                }
            }
            //cursorMoveEvent.Invoke(move.storedRotation);
            move.hasBeenChecked = true;
            //queuedCursorMoves.Remove(move);
        }

        queuedCursorMoves.RemoveAll(x => x.hasBeenChecked == true);

        foreach (StoredPlayerFire fire in queuedPlayerFires)
        {
            foreach (PhoneCursor user in users)
            {
                if (user.userID == fire.userID)
                {
                    user.Fire(webSocket);
                }
            }
            //fireEvent.Invoke(webSocket);
            fire.hasBeenChecked = true;
        }
        queuedPlayerFires.RemoveAll(x => x.hasBeenChecked == true);

        for (int i = queuedUsers.Count - 1; i >= 0; i--)
        {
            GameObject phoneCursor = Instantiate(phoneCursorPrefab);
            PhoneCursor phoneComp = phoneCursor.GetComponent<PhoneCursor>();
            phoneComp.userID = queuedUsers[i];
            //Add user to list
            users.Add(phoneComp);
            queuedUsers.RemoveAt(i);
        }

        for (int i = queuedDisconnects.Count - 1; i >= 0; i--)
        {
            foreach (PhoneCursor user in users)
            {
                if (user.userID == queuedDisconnects[i])
                {
                    GameObject.Destroy(user.gameObject);
                }
            }
            queuedDisconnects.RemoveAt(i);
        }
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
                queuedCursorMoves.Add(new StoredCursorMove(ourVector, rotMsg.id));
                break;
            case "fire":
                FireMessage fireMsg = JsonUtility.FromJson<FireMessage>(e.Data);
                queuedPlayerFires.Add(new StoredPlayerFire(fireMsg.id));
                break;
            case "userConnect":
                UserConnectMessage userMsg = JsonUtility.FromJson<UserConnectMessage>(e.Data);
                queuedUsers.Add(userMsg.id);
                break;
            case "userDisconnect":
                UserConnectMessage userDCMsg = JsonUtility.FromJson<UserConnectMessage>(e.Data);
                queuedDisconnects.Add(userDCMsg.id);             
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
