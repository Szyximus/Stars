using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.UI;

public class LoadGameSceneInit : MonoBehaviour
{
    private GameApp gameApp;
    private LevelLoader levelLoader;
    private RectTransform dynamicGrid;
    private List<GameObject> playersToAddToGame;

    void Start()
    {
        Debug.Log("LoadGameSceneInit");

        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        dynamicGrid = GameObject.Find("ListCanvas/ListPanel/ListGrid").GetComponent<RectTransform>();

        playersToAddToGame = new List<GameObject>();

        string path = gameApp.savedGamesPath + gameApp.GetInputField("GameToLoad");
        JObject gameParsed = null;
        try
        {
            gameParsed = gameApp.ReadJsonFile(path);
        }
        catch(Exception e)
        {
            Debug.Log("LoadGameSceneInit error: " + e.Message);
            Back();
        }

        if (gameParsed == null)
        {
            Debug.Log("LoadGameSceneInit gameParsed is null");
            Back();
        }

        JArray playersJson;
        int maxPlayers;
        try
        {
            playersJson = (JArray)gameParsed["players"];
            maxPlayers = (int)gameParsed["info"]["maxPlayers"];
        }
        catch (Exception e)
        {
            Debug.Log("LoadGameSceneInit error: " + e.Message);
            Back();
            return;
        }

        if (maxPlayers != playersJson.Count)
        {
            Debug.Log("LoadGameSceneInit error: maxPlayers != playersJson.Count");
            Back();
            return;
        }

        foreach (JObject playerJson in playersJson)
        {
            GameObject newPlayer = Instantiate(gameApp.PlayerMenuPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            newPlayer.transform.SetParent(dynamicGrid.transform, false);
            newPlayer.transform.Find("PlayerNameInput").GetComponent<InputField>().text = (string)playerJson["name"];
            newPlayer.transform.Find("PlayerNameInput").GetComponent<InputField>().enabled = false;

            newPlayer.transform.Find("PlayerRaceInput").GetComponent<InputField>().text = (string)playerJson["playerMain"]["race"];
            newPlayer.transform.Find("PlayerRaceInput").GetComponent<InputField>().enabled = false;
            playersToAddToGame.Add(newPlayer);
        }
    }

    public void Back()
    {
        levelLoader.LoadLevel("LoadGameMapScene");
    }

    public void Create()
    {
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        foreach (GameObject player in playersToAddToGame)
        {
            playerMenuList.Add(new GameApp.PlayerMenu
            {
                name = player.transform.Find("PlayerNameInput").GetComponent<InputField>().text,
                password = player.transform.Find("PlayerPassInput").GetComponent<InputField>().text,
                race = player.transform.Find("PlayerRaceInput").GetComponent<InputField>().text,
                playerType = player.transform.Find("PlayerTypeInput").GetComponent<InputField>().text,
            });
        }

        Debug.Log("LoadGameSceneInit create, players: " + playerMenuList.Count);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupLoadGame();
    }
}
