using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

public class NewGameSceneInit : MonoBehaviour
{
    private GameApp gameApp;
    private LevelLoader levelLoader;
    private RectTransform dynamicGrid;
    private List<GameObject> playersToAddToGame;
    private int maxPlayers;

    void Start()
    {
        Debug.Log("NewGameSceneInit");

        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        dynamicGrid = GameObject.Find("ListCanvas/ListPanel/ListGrid").GetComponent<RectTransform>();

        playersToAddToGame = new List<GameObject>();

        string path = gameApp.startMapsPath + gameApp.GetInputField("MapToLoad");
        JObject gameParsed = null;
        try
        {
            gameParsed = gameApp.ReadJsonFile(path);
        }
        catch (Exception e)
        {
            Debug.Log("NewGameSceneInit error: " + e.Message);
            Back();
        }

        if (gameParsed == null)
        {
            Debug.Log("NewGameSceneInit gameParsed is null");
            Back();
        }

        try
        {
            maxPlayers = (int)gameParsed["info"]["maxPlayers"];
        } catch(Exception e)
        {
            Debug.Log("NewGameSceneInit error: " + e.Message);
            Back();
        }

    }

    public void Back()
    {
        levelLoader.LoadLevel("NewGameMapScene");
    }

    public void Create()
    {
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        foreach (GameObject player in playersToAddToGame)
        {
            playerMenuList.Add( new GameApp.PlayerMenu {
                name = player.transform.Find("PlayerNameInput").GetComponent<InputField>().text,
                password = player.transform.Find("PlayerPassInput").GetComponent<InputField>().text,
                race = player.transform.Find("PlayerRaceInput").GetComponent<InputField>().text,
                playerType = player.transform.Find("PlayerTypeInput").GetComponent<InputField>().text,
            });
        }

        Debug.Log("NewGameSceneInit create, players: " + playerMenuList.Count);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
    }

    public void AddPlayer()
    {
        if(playersToAddToGame.Count >= maxPlayers)
        {
            Debug.Log("NewGameSceneInit, max players reached");
            return;
        }
        GameObject newPlayer = Instantiate(gameApp.PlayerMenuPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        newPlayer.transform.SetParent(dynamicGrid.transform, false);
        playersToAddToGame.Add(newPlayer);
    }

    public void RemovePlayer()
    {
        if(playersToAddToGame.Count <= 0)
        {
            return;
        }
        Destroy(playersToAddToGame[playersToAddToGame.Count - 1]);
        playersToAddToGame.RemoveAt(playersToAddToGame.Count - 1);
    }
}
