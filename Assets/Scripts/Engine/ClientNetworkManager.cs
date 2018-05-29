using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;

public class ClientNetworkManager : NetworkManager
{

    private GameController gameController;
    private GameApp gameApp;
    public NetworkClient networkClient;

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
    }

    public void SetupClient()
    {
        Debug.Log("SetupClient");

        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters("JoinGameScene");
        this.networkAddress = gameApp.GetAndRemoveInputField("ServerAddress");
        this.networkPort = int.Parse(gameApp.GetAndRemoveInputField("ServerPort"));

        this.networkAddress = "192.168.1.10";
        this.networkPort = 7777;
        this.StartClient();
    }

    // Client callbacks
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnClientSceneChanged: " + conn);
        base.OnClientSceneChanged(conn);

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.clientNetworkManager = this;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("Connected successfully to server");

        NetworkServer.RegisterHandler(GameApp.connAssignPlayerError, OnClientAssignPlayerError);
        NetworkServer.RegisterHandler(GameApp.connAssignPlayerSuccess, OnClientAssignPlayerSuccess);

        string playerName = gameApp.GetAndRemoveInputField("PlayerName");
        string password = gameApp.GetAndRemoveInputField("Password");

        StringMessage playerMsg = new StringMessage(playerName);
        networkClient.Send(GameApp.connAssignPlayerId, playerMsg);
    }

    public void OnClientAssignPlayerError(NetworkMessage netMsg)
    {
        Debug.Log("OnClientAssignPlayerError: " + netMsg.ReadMessage<StringMessage>());
    }

    public void OnClientAssignPlayerSuccess(NetworkMessage netMsg)
    {
        Debug.Log("OnClientAssignPlayerSuccess: " + netMsg.ReadMessage<StringMessage>());
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();

        if (conn.lastError != NetworkError.Ok)
        {
            if (LogFilter.logError) { Debug.LogError("ClientDisconnected due to error: " + conn.lastError); }
        }

        Debug.Log("Client disconnected from server: " + conn);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {

        Debug.Log("Client network error occurred: " + (NetworkError)errorCode);

    }

    public override void OnClientNotReady(NetworkConnection conn)
    {

        Debug.Log("Server has set client to be not-ready (stop getting state updates): " + conn);
        //gameController.WaitForTurn();
    }

    public override void OnStartClient(NetworkClient client)
    {

        Debug.Log("Client has started");
        networkClient = client;
    }

    public override void OnStopClient()
    {

        Debug.Log("Client has stopped");

    }

}