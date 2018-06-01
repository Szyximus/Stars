using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.Networking.NetworkSystem;
using Newtonsoft.Json.Linq;

/*
 *  This is singleton object
 *  Created at "NewGameScene" or "LoadGameScene"
 *  Destroyed at game exit (in GameController.Exit) or "Back" button in "NewGameScene" and "LoadGameScene" (in LevelLoader.Back)
 */
public class ServerNetworkManager : NetworkManager
{
    private GameController gameController;
    private GameApp gameApp;
    private LevelLoader levelLoader;

    private bool created = false;

    // these vars are used at scene change, which may be after game creation, game loading or next turn from remote client
    private bool isNewGame;
    private bool isLoadGame;
    private string nextTurnGameJson;

    // dict with player name -> remote client connection
    public Dictionary<string, NetworkConnection> connections;


    void Awake()
    {
        if (!created)
        {
            connections = new Dictionary<string, NetworkConnection>();

            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();

            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
    }

    /*
     *  After "Create" button in "NewGameScene"
     *  Setup bind address and port, start server, change scene
     */
    public void SetupNewGame()
    {
        Debug.Log("SetupNewGame");
        isNewGame = true;
        isLoadGame = false;

        try
        {
            // persists config data from menu scene
            gameApp.PersistAllParameters("NewGameScene");

            this.networkAddress = gameApp.GetAndRemoveInputField("Address");
            this.networkPort = int.Parse(gameApp.GetAndRemoveInputField("Port"));
        } catch(Exception e)
        {
            Debug.Log("SetupNewGame error: " + e.Message);
            gameApp.RemoveAllParameters();
            return;
        }

        this.networkAddress = "127.0.0.1";
        this.networkPort = 7777;
        this.StartServer();
        this.ServerChangeScene("GameScene");
    }

    /*
     *   After "Load" button in "LoadGameScene"
     *   Setup bind address and port, start server, change scene
     *   
     */
    public void SetupLoadGame()
    {
        Debug.Log("SetupLoadGame");
        isNewGame = false;
        isLoadGame = true;

        try
        {
            // persists config data from menu scene
            gameApp.PersistAllParameters("LoadGameScene");

            this.networkAddress = gameApp.GetAndRemoveInputField("Address");
            this.networkPort = int.Parse(gameApp.GetAndRemoveInputField("Port"));
        } catch(Exception e)
        {
            Debug.Log("SetupLoadGame error: " + e.Message);
            gameApp.RemoveAllParameters();
            return;
        }

        this.networkAddress = "192.168.1.10";
        this.networkPort = 7777;
        this.StartServer();
        this.ServerChangeScene("GameScene");
    }

    /*
     *  Called from "OnServerClientNextTurnDone"
     *  Change scene with json received from remote client
     */
    public void SetupNextTurn(string clientGameJson)
    {
        Debug.Log("NextTurnScene");
        isNewGame = false;
        isLoadGame = false;

        nextTurnGameJson = clientGameJson;

        // this should set all clients to "not ready" state
        this.ServerChangeScene("GameScene");
    }


    // Server callbacks

    /*
     *  After server was started or received next turn from remote client
     *  Scene should have been changed to "GameScene", GameController should be available
     *  Calls initialization function from GameController
     */
    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("OnServerSceneChanged: " + sceneName);
        base.OnServerSceneChanged(sceneName);

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.serverNetworkManager = this;

        if (isNewGame)
        {
            // create new game
            List<GameApp.PlayerMenu> PlayerMenuList = gameApp.GetAllPlayersFromMenu();

            try
            {
                gameController.ServerStartNewGame(PlayerMenuList);
            }
            catch (Exception e)
            {
                Debug.Log("OnServerSceneChanged gameController.ServerStartNewGame error: " + e.Message);
                this.StopServer();
            }
        }
        else if(isLoadGame)
        {
            // load game from json file. Read it here, so we can use ServerLoadGame as in nextTurn from remote client
            string savedGameFile = gameApp.GetAndRemoveInputField("SavedGameFile");
            if (savedGameFile == null || savedGameFile.Equals(""))
            {
                Debug.Log("savedGameFile empty");
                this.StopServer();
                return;
            }

            string path = gameApp.savedGamesPath + "/" + savedGameFile + ".json";
            StreamReader reader = new StreamReader(path);
            string savedGameContent = reader.ReadToEnd();
            reader.Close();

            if (savedGameContent == null || "".Equals(savedGameContent))
            {
                Debug.Log("savedGameContent is null, path: " + path);
                this.StopServer();
                return;
            }

            try
            {
                gameController.ServerLoadGame(savedGameContent);
            } catch(Exception e)
            {
                Debug.Log("OnServerSceneChanged gameController.ServerLoadGame error: " + e.Message);
                this.StopServer();
                return;
            }
        }
        else
        {
            try
            {
                // next turn from remote client
                gameController.ServerLoadGame(nextTurnGameJson);
                nextTurnGameJson = null;
            } catch(Exception e)
            {
                Debug.Log("OnServerSceneChanged gameController.ServerLoadGame error: " + e.Message);
            }
        }
    }

    /*
     *   Custom callback, invoked from "OnClientConnect", when the client connected to the server and want join the game.
     *   Server validate player name etc. and send either connAssignPlayerErrorId, connAssignPlayerSuccessId or connClientReadyId
     */
    public void OnServerClientAssignPlayer(NetworkMessage netMsg)
    {
        var clientPlayerNameMsg = netMsg.ReadMessage<StringMessage>();
        if(clientPlayerNameMsg == null)
        {
            Debug.Log("OnServerClientAssignPlayer, clientPlayerNameMsg is null ");
            netMsg.conn.Send(GameApp.connAssignPlayerErrorId, new StringMessage("clientPlayerNameMsg is null"));
            return;
        }

        string clientPlayerName = clientPlayerNameMsg.value;
        Debug.Log("OnServerClientAssignPlayer, player name: " + clientPlayerName);


        if (connections.ContainsKey(clientPlayerName))
        {
            Debug.Log("OnServerClientAssignPlayer: player taken");
            netMsg.conn.Send(GameApp.connAssignPlayerErrorId, new StringMessage("Player is taken"));
        }
        else if (gameController.FindPlayer(clientPlayerName) == null)
        {
            Debug.Log("OnServerClientAssignPlayer: player name not found, " + clientPlayerName);
            netMsg.conn.Send(GameApp.connAssignPlayerErrorId, new StringMessage("Player with name " + clientPlayerName + " not found"));
        }
        else
        {
            Debug.Log("OnServerClientAssignPlayer: player joined, " + clientPlayerName);
            connections.Add(clientPlayerName, netMsg.conn);

            if (clientPlayerName.Equals(GameController.GetCurrentPlayer().name))
            {
                // client that just joined game shoud make turn now
                Debug.Log("OnServerClientAssignPlayer, it is new client turn");
                NetworkServer.SetClientReady(netMsg.conn);
                NetworkServer.SendToClient(netMsg.conn.connectionId, GameApp.connClientReadyId, new EmptyMessage());
            } else
            {
                // client should wait for his turn (one of this shoud be enough actually)
                NetworkServer.SetClientNotReady(netMsg.conn);
                netMsg.conn.Send(GameApp.connAssignPlayerSuccessId, new StringMessage("Player with name " + clientPlayerName + " assigned"));
            }
        }
    }

    /*
     *  Custom callback, invoked from remote client at the and of the turn ("NextTurn" button)
     *  Contains message with serialized map (as json)
     *  Validate the message and setup new turn if valid
     */
    public void OnServerClientNextTurnDone(NetworkMessage netMsg)
    {
        // here we validate map from the client
        var clientGameJsonMsg = netMsg.ReadMessage<StringMessage>();
        if(clientGameJsonMsg == null)
        {
            Debug.Log("OnServerClientNextTurnDone: clientMapJsonMsg is null");
            return;
        }

        string clientGameJson = clientGameJsonMsg.value;
        Debug.Log("OnServerClientNextTurnDone, game: " + clientGameJson);

        string playerName = null;
        foreach (var playerConnPair in connections)
        {
            if(playerConnPair.Value.Equals(netMsg.conn))
            {
                playerName = playerConnPair.Key;
                break;
            }
        }

        if (playerName == null)
        {
            Debug.Log("OnServerClientNextTurnDone: connection not found");
            return;
        }

        if(!playerName.Equals(GameController.GetCurrentPlayer().name))
        {
            Debug.Log("OnServerClientNextTurnDone: current player is " + GameController.GetCurrentPlayer().name + ", not " + playerName);
            return;
        }

        JObject clientGameJsonParsed = JObject.Parse(clientGameJson);
        if (clientGameJsonParsed == null)
        {
            Debug.Log("OnServerClientNextTurnDone: error loading json, clientGameJsonParsed is null");
            return;
        }



        SetupNextTurn(clientGameJson);
    }

    /*
     *  Server started, register handlers
     */
    public override void OnStartServer()
    {
        Debug.Log("Server has started");

        // invoked when remote cliend made turn
        NetworkServer.RegisterHandler(GameApp.connMapJsonId, OnServerClientNextTurnDone);

        // invoked when remote client connected and wants join the game
        NetworkServer.RegisterHandler(GameApp.connAssignPlayerId, OnServerClientAssignPlayer);
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

    /*
     *  We do not use networking players
     */
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

    /*
     *  We run server is "server mode" and handle local players with custom code
     */
    public override void OnStartHost()
    {
        Debug.Log("Host has started");
    }

    /*
     *  Server stopped, return to main menu
     */
    public override void OnStopServer()
    {
        Debug.Log("Server has stopped");
        levelLoader.Back("MainMenuScene");
    }

    public override void OnStopHost()
    {
        Debug.Log("Host has stopped");
    }

}