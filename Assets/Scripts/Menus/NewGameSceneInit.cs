/*
    Copyright (c) 2018, Szymon Jakóbczyk, Paweł Płatek, Michał Mielus, Maciej Rajs, Minh Nhật Trịnh, Izabela Musztyfaga
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation 
          and/or other materials provided with the distribution.
        * Neither the name of the [organization] nor the names of its contributors may be used to endorse or promote products derived from this software 
          without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
    USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*
 *  Class used in NewGameScene
 */
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

        // get and parse map to load
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
        levelLoader.Back("NewGameMapScene");
    }

    /*
     *  Get players' data from the menu input fields and pass to the gameApp
     *  So the data can be read by gameController afterwards
     */
    public void Create()
    {
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        foreach (GameObject player in playersToAddToGame)
        {
            string tempType;
            if (player.transform.Find("PlayerTypeInput").GetComponent<Toggle>().isOn)
            {
                tempType = "L";
            }
            else tempType = "R";
            playerMenuList.Add( new GameApp.PlayerMenu {
                name = player.transform.Find("PlayerNameInput").GetComponent<InputField>().text,
                password = player.transform.Find("PlayerPassInput").GetComponent<InputField>().text,
                race = "human",
                playerType = tempType
            });
        }

        Debug.Log("NewGameSceneInit create, players: " + playerMenuList.Count);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
    }

    /*
     *  Add input for a player to the menu
     */
    public void AddPlayer()
    {
        if(playersToAddToGame.Count >= maxPlayers)
        {
            Debug.Log("NewGameSceneInit, max players reached");
            return;
        }
        GameObject newPlayer = Instantiate(gameApp.PlayerMenuPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        newPlayer.transform.SetParent(dynamicGrid.transform, false);
        EventTrigger trigger = newPlayer.GetComponentInChildren<Toggle>().gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((eventData) => {
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("EventSystem"));
        });
        trigger.triggers.Add(entry);

        playersToAddToGame.Add(newPlayer);
        
    }

    /*
     *  Remove player from the menu
     */
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
