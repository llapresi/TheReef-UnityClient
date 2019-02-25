using UnityEngine;
using UnityEngine.Events;
using System;
using WebSocketSharp;
using System.Collections.Generic;
using System.Linq;

public class StoredPlayerFire
{
    public int userID;
    public bool isHeld;

    public StoredPlayerFire(int p_id, bool p_isHeld)
    {
        userID = p_id;
        isHeld = p_isHeld;
    }
}

public class StoredCursorMove
{
    public Vector3 storedRotation;
    public int userID;

    public StoredCursorMove(Vector3 p_storedRotation, int p_id)
    {
        storedRotation = p_storedRotation;
        userID = p_id;
    }
}

public class SocketTest : MonoBehaviour {

    public string serverURL;
    WebSocket webSocket;
    public Camera camera;
    public List<PhoneCursor> users;
    public GameObject phoneCursorPrefab;

    //List<StoredCursorMove> queuedCursorMoves;
    //List<StoredPlayerFire> queuedPlayerFires;
    //List<int> queuedUsers;
    //List<int> queuedDisconnects;

    // Newer threaded queues
    ThreadLockedQueue<int> queuedUsers;
    ThreadLockedQueue<int> queuedDisconnects;

    ThreadLockedQueue<StoredCursorMove> queuedCursorMoves;
    ThreadLockedQueue<StoredPlayerFire> queuedPlayerFires;


    // Use this for initialization
    void Start()
    {
        // Init non socket connection related stuff
        queuedPlayerFires = new ThreadLockedQueue<StoredPlayerFire>();
        queuedCursorMoves = new ThreadLockedQueue<StoredCursorMove>();
        queuedUsers = new ThreadLockedQueue<int>();
        queuedDisconnects = new ThreadLockedQueue<int>();
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
            webSocket.Connect();
        }
    }

    private void Update() {
        // Set the rotation (used to drive the cursor)

        //debug line
        //Color color = new Color(0.0f, 0.0f, 1.0f);
        //Debug.DrawLine(Vector3.zero, aimForward * 20.0f, Color.green, 2.0f);

        while(queuedCursorMoves.Count() > 0)
        {
            StoredCursorMove move = queuedCursorMoves.Dequeue();
            foreach (PhoneCursor user in users)
            {
                if (user.userID == move.userID)
                {
                    user.SetStoredRotation(move.storedRotation);
                }
            }
        }

        while(queuedPlayerFires.Count() > 0)
        {
            StoredPlayerFire fire = queuedPlayerFires.Dequeue();
            //Debug.Log("Fire Msg | IsHeld: " + fire.isHeld + " | User ID: " + fire.userID);
            foreach (PhoneCursor user in users)
            {
                if (user.userID == fire.userID)
                {
                    if (!fire.isHeld)
                    {
                       user.Release(webSocket);
                    }
                    else
                    {
                       user.Fire(webSocket);
                    }

                }
            }
        }

        while(queuedUsers.Count() > 0)
        {
            GameObject phoneCursor = Instantiate(phoneCursorPrefab);
            PhoneCursor phoneComp = phoneCursor.GetComponent<PhoneCursor>();
            phoneComp.userID = queuedUsers.Dequeue();
            //Add user to list
            users.Add(phoneComp);
        }

        while(queuedDisconnects.Count() > 0)
        {
            int disconnectedUser = queuedDisconnects.Dequeue();
            for (int q = users.Count - 1; q >= 0; q--)
            {
                PhoneCursor user = users[q];
                if (user.userID == disconnectedUser)
                {
                    users.Remove(user);
                    GameObject.Destroy(user.gameObject);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if(webSocket.IsAlive)
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
                queuedCursorMoves.Enqueue(new StoredCursorMove(ourVector, rotMsg.id));
                break;
            case "fire":
                FireMessage fireMsg = JsonUtility.FromJson<FireMessage>(e.Data);
                queuedPlayerFires.Enqueue(new StoredPlayerFire(fireMsg.id, fireMsg.held));
                break;
            case "userConnect":
                UserConnectMessage userMsg = JsonUtility.FromJson<UserConnectMessage>(e.Data);
                queuedUsers.Enqueue(userMsg.id);
                break;
            case "userDisconnect":
                UserConnectMessage userDCMsg = JsonUtility.FromJson<UserConnectMessage>(e.Data);
                queuedDisconnects.Enqueue(userDCMsg.id);             
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
