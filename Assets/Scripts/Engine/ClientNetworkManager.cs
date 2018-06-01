using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;
using System;

/*
 *  This is singleton object
 *  Created at "JoinGameScene"
 *  Destroyed at game exit or "Back" button in "JoinGameScene" (in LevelLoader.Back)
 */
public class ClientNetworkManager : NetworkManager
{

    private GameController gameController;
    private GameApp gameApp;
    private LevelLoader levelLoader;

    public NetworkClient networkClient;
    public NetworkConnection connection;

    private bool created = false;

    void Awake()
    {
        if (!created)
        {
            gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
    }

    /*
     *   After "Join" button in "JoinGameScene"
     *   Setup server address and port, start client
     */
    public void SetupClient()
    {
        Debug.Log("SetupClient");

        try
        {
            // persists config data from menu scene
            gameApp.PersistAllParameters("JoinGameScene");

            this.networkAddress = gameApp.GetAndRemoveInputField("ServerAddress");
            this.networkPort = int.Parse(gameApp.GetAndRemoveInputField("ServerPort"));
        } catch(Exception e)
        {
            Debug.Log("SetupClient error: " + e.Message);
            gameApp.RemoveAllParameters();
            return;
        }

        this.networkAddress = "127.0.0.1";
        this.networkPort = 7777;
        this.StartClient();
    }

    // Client callbacks

    /*
     *  After connection to the server, scene should changed to "GameScene"
     *  After the change, server is informed that client is ready and he spawns objects
     *  Then server calls 
     */
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnClientSceneChanged: " + conn);
        base.OnClientSceneChanged(conn);
    }

    /*
     *  Invoked when client connects to the server
     *  It sends message with player name (connAssignPlayerId), server should then send connAssignPlayerErrorId, connAssignPlayerSuccessId or connClientReadyId
     */
    public override void OnClientConnect(NetworkConnection conn)
    {
        //base.OnClientConnect(conn);
        Debug.Log("OnClientConnect: Connected successfully to server");

        ClientScene.RegisterPrefab(gameApp.PlayerPrefab);
        ClientScene.RegisterPrefab(gameApp.PlanetPrefab);
        ClientScene.RegisterPrefab(gameApp.StartPrefab);
        ClientScene.RegisterPrefab(gameApp.ScoutPrefab);
        ClientScene.RegisterPrefab(gameApp.ColonizerPrefab);
        ClientScene.RegisterPrefab(gameApp.MinerPrefab);
        ClientScene.RegisterPrefab(gameApp.WarshipPrefab);
        ClientScene.RegisterPrefab(gameApp.ExplosionPrefab);
        ClientScene.RegisterPrefab(gameApp.AttackPrefab);
        ClientScene.RegisterPrefab(gameApp.HitPrefab);

        networkClient.RegisterHandler(gameApp.connAssignPlayerErrorId, OnClientAssignPlayerError);
        networkClient.RegisterHandler(gameApp.connAssignPlayerSuccessId, OnClientAssignPlayerSuccess);
        networkClient.RegisterHandler(gameApp.connSetupTurnId, OnClientSetupTurn);
        networkClient.RegisterHandler(gameApp.connClientLoadGameId, OnClientLoadGame);

        string playerName = gameApp.GetAndRemoveInputField("PlayerName");
        string password = gameApp.GetAndRemoveInputField("Password");

        Debug.Log("OnClientConnect: sending player name: " + playerName);
        StringMessage playerMsg = new StringMessage(playerName);
        networkClient.Send(gameApp.connAssignPlayerId, playerMsg);
    }


    /*
     *  Custom callback (on connAssignPlayerErrorId)
     *  Server invoke it when client can't join the game
     *  May be wrong player name, or the player is taken already
     */
    public void OnClientAssignPlayerError(NetworkMessage netMsg)
    {
        Debug.Log("OnClientAssignPlayerError: " + netMsg.ReadMessage<StringMessage>().value);
        netMsg.conn.Disconnect();
    }

    /*
     * Custom callback (on connAssignPlayerSuccessId)
     * Server invoke it when client joinned to the game
     * Client will wait for the turn
     */
    public void OnClientAssignPlayerSuccess(NetworkMessage netMsg)
    {
        Debug.Log("OnClientAssignPlayerSuccess: " + netMsg.ReadMessage<StringMessage>().value);
    }


    /*
     *  Custom callback (on connSetupTurnId)
     *  Server invoke it when it is this client's turn
     *  Client set to "ready" state and start plays the game
     */
    public void OnClientSetupTurn(NetworkMessage netMsg)
    {
        Debug.Log("OnClientSetupTurn");

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        bool playNow = netMsg.ReadMessage<IntegerMessage>().value == 1;
        if (playNow)
            gameController.StopWaitForTurn();
        else
            gameController.WaitForTurn();
    }

    public void OnClientLoadGame(NetworkMessage netMsg)
    {
        Debug.Log("OnClientLoadGame");

        string savedGame = netMsg.ReadMessage<StringMessage>().value;

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.ClientNextTurnGame(savedGame);
    }

    /*
     *  Called in GameController after next turn
     *  Client should wait
     */
    public override void OnClientNotReady(NetworkConnection conn)
    {
        Debug.Log("Server has set client to be not-ready (stop getting state updates): " + conn);
        gameController.WaitForTurn();
    }


    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();
        if (conn.lastError != NetworkError.Ok)
        {
            if (LogFilter.logError) { Debug.LogError("ClientDisconnected due to error: " + conn.lastError); }
        }
        Debug.Log("Client disconnected from server: " + conn);

        levelLoader.Back("MainMenuScene");
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Client network error occurred: " + (NetworkError)errorCode);
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