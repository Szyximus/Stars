using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    }

    public override void OnClientConnect(NetworkConnection conn)

    {
        base.OnClientConnect(conn);
        Debug.Log("Connected successfully to server");
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