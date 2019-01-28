using UnityEngine;
using System;
using WebSocketSharp;

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
    WebSocket webSocket;
    

    // Use this for initialization
    void Start()
    {
        webSocket = new WebSocket(serverURL);

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

    private void Update() { }

    private void OnDestroy()
    {
        if (webSocket != null)
        {
            webSocket.Close();
        }
    }

    private void OnOpenHandler(object sender, EventArgs e)
    {
        Debug.Log("OnOpenHandler -> Open socket: " + webSocket.ReadyState + " - Websocket Alive: " + webSocket.IsAlive);
    }

    private void OnMessageHandler(object sender, MessageEventArgs e)
    {
        Debug.Log("OnMessageHandler | " + e.Data);
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
