using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.Networking.NetworkSystem;

public class ServerNetworkManager : NetworkManager
{
    private GameController gameController;
    private GameApp gameApp;

    private static bool created = false;
    private bool isNewGame;
    private bool isNextTurnGame;
    private string nextTurnJson;

    public Dictionary<string, NetworkConnection> connections;

    void Awake()
    {
        if (!created)
        {
            connections = new Dictionary<string, NetworkConnection>();

            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
    }

    public void SetupNewGame()
    {
        Debug.Log("SetupNewGame");
        isNewGame = true;

        try
        {
            gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
            gameApp.PersistAllParameters("NewGameScene");
            this.networkAddress = gameApp.GetAndRemoveInputField("Address");
            this.networkPort = int.Parse(gameApp.GetAndRemoveInputField("Port"));
        } catch(Exception)
        {
            gameApp.RemoveAllParameters();
        }

        this.networkAddress = "192.168.1.10";
        this.networkPort = 7777;
        this.StartServer();
        this.ServerChangeScene("GameScene");
    }

    public void SetupLoadGame()
    {
        Debug.Log("SetupLoadGame");
        isNewGame = false;
        isNextTurnGame = false;

        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters("NewGameScene");
        this.networkAddress = gameApp.GetAndRemoveInputField("Address");
        this.networkPort = int.Parse(gameApp.GetAndRemoveInputField("Port"));

        this.networkAddress = "192.168.1.10";
        this.networkPort = 7777;
        this.StartServer();
        this.ServerChangeScene("GameScene");
    }

    public void NextTurnScene(string clientMapJson)
    {
        Debug.Log("NextTurnScene");
        nextTurnJson = clientMapJson;
        isNewGame = false;
        isNextTurnGame = true;
        this.ServerChangeScene("GameScene");
    }

    // Server callbacks

    public void OnServerClientAssignPlayer(NetworkMessage netMsg)
    {
        var clientPlayerName = netMsg.ReadMessage<StringMessage>();
        Debug.Log("received OnServerClientAssignPlayer " + clientPlayerName.value);

        if(connections.ContainsKey(clientPlayerName.value))
        {
            Debug.Log("OnServerClientAssignPlayer: player taken");
            netMsg.conn.Send(GameApp.connAssignPlayerError, new StringMessage("Player is taken"));
            netMsg.conn.Disconnect();
        }
        else if(gameController.FindPlayer(clientPlayerName.value) == null)
        {
            Debug.Log("OnServerClientAssignPlayer: player name not found, " + clientPlayerName.value);
            netMsg.conn.Send(GameApp.connAssignPlayerError, new StringMessage("Player with name " + clientPlayerName.value + " not found"));
            netMsg.conn.Disconnect();
        }
        else
        {
            Debug.Log("OnServerClientAssignPlayer: player join, " + clientPlayerName.value);
            netMsg.conn.Send(GameApp.connAssignPlayerSuccess, new StringMessage("Player with name " + clientPlayerName.value + " assigned"));
            connections.Add(clientPlayerName.value, netMsg.conn);

            if (clientPlayerName.value.Equals(GameController.GetCurrentPlayer().name))
            {
                Debug.Log("OnServerClientAssignPlayer, it is new client turn");
                NetworkServer.SetClientReady(netMsg.conn);
            }
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("OnServerSceneChanged: " + sceneName);
        base.OnServerSceneChanged(sceneName);

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.serverNetworkManager = this;

        if (isNewGame)
        {
            List<GameApp.PlayerMenu> PlayerMenuList = gameApp.GetAllPlayersFromMenu();
            gameController.ServerStartNewGame(PlayerMenuList);
        }
        else if(!isNextTurnGame)
        {
            string savedGameFile = gameApp.GetAndRemoveInputField("SavedGameFile");
            if (savedGameFile == null || savedGameFile.Equals(""))
            {
                Debug.Log("savedGameFile empty");
                this.StopServer();
                return;
            }

            string path = gameApp.configsPath + "/" + savedGameFile + ".json";
            StreamReader reader = new StreamReader(path);
            string savedGameContent = reader.ReadToEnd();
            reader.Close();

            if (savedGameContent == null || "".Equals(savedGameContent))
            {
                Debug.Log("savedGameContent is null, path: " + path);
                this.StopServer();
                return;
            }

            gameController.ServerLoadGame(savedGameContent);
        }
        else
        {
            gameController.ServerLoadGame(nextTurnJson);
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("A client connected to the server: " + conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {

        NetworkServer.DestroyPlayersForConnection(conn);

        if (conn.lastError != NetworkError.Ok)
        {

            if (LogFilter.logError) { Debug.LogError("ServerDisconnected due to error: " + conn.lastError); }

        }

        Debug.Log("A client disconnected from the server: " + conn);

    }

    public override void OnServerReady(NetworkConnection conn)
    {

        NetworkServer.SetClientReady(conn);

        Debug.Log("Client is set to the ready state (ready to receive state updates): " + conn);

    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {

       // var player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

       // NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        Debug.Log("Client has requested to get his player added to the game");


    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {

       // if (player.gameObject != null)

         //   NetworkServer.Destroy(player.gameObject);

    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {

        Debug.Log("Server network error occurred: " + (NetworkError)errorCode);

    }

    public override void OnStartHost()
    {

        Debug.Log("Host has started");

    }

    public override void OnStartServer()
    {
        Debug.Log("Server has started");
        NetworkServer.RegisterHandler(GameApp.connAssignPlayerId, OnServerClientAssignPlayer);
    }

    public override void OnStopServer()
    {

        Debug.Log("Server has stopped");

    }

    public override void OnStopHost()
    {

        Debug.Log("Host has stopped");

    }

}