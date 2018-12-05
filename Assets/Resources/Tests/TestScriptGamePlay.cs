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
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.HexLogic;
using System;

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

public class TestScriptGamePlay
{
    [UnityTest]
    public IEnumerator TestYear_NoNextTurn_2004Expected()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9983");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(3);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
   
        Assert.AreEqual(gameController.year, 2400);
    }

    [UnityTest]
    public IEnumerator TestYear_Next1Turns_2400Expected()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9982");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(3);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.NextTurn();
        yield return new WaitForSeconds(3);

        Assert.AreEqual(gameController.year, 2400);
    }

    [UnityTest]
    public IEnumerator TestYear_Next2Turns_2401Expected()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9981");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(3);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.NextTurn();
        yield return new WaitForSeconds(3);
        gameController.NextTurn();
        yield return new WaitForSeconds(3);

        Assert.AreEqual(gameController.year, 2401);
    }

    [UnityTest]
    public IEnumerator TestYear_Next3Turns_2401Expected() {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9980");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(3);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.NextTurn();
        yield return new WaitForSeconds(3);
        gameController.NextTurn();
        yield return new WaitForSeconds(3);
        gameController.NextTurn();
        yield return new WaitForSeconds(3);

        Assert.AreEqual(gameController.year, 2401);
    }

    [UnityTest]
    public IEnumerator TestMovementOfSpaceShip_Require1Move_Move1Cell()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9987");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(1);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject gameObjectspaceShip = GameObject.Find("Scout-1");
        Spaceship spaceShip = gameObjectspaceShip.GetComponent<Spaceship>();
        HexGrid grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        var des = new HexCoordinates(spaceShip.Coordinates.X, spaceShip.Coordinates.Z + 1);
        EventManager.selectionManager.SelectedObject = gameObjectspaceShip;
        yield return new WaitForSeconds(3);
        grid.TouchCell(des);
        yield return new WaitForSeconds(3);

        Assert.AreEqual(spaceShip.Coordinates, des);
    }

    [UnityTest]
    public IEnumerator TestMovementOfSpaceShip_Require3Moves_3MovesExpected()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9996");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(1);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject gameObjectspaceShip = GameObject.Find("Scout-1");
        Spaceship spaceShip = gameObjectspaceShip.GetComponent<Spaceship>();
        HexGrid grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        Vector3 vector = new Vector3(310.0f, 0.0f, 270.0f);
        EventManager.selectionManager.SelectedObject = gameObjectspaceShip;
        yield return new WaitForSeconds(3);
        grid.TouchCell(vector);
        yield return new WaitForSeconds(4);

        HexCoordinates expected = HexCoordinates.FromPosition(vector);
        Assert.AreEqual(spaceShip.Coordinates, expected);
    }

    [UnityTest]
    public IEnumerator TestCreatingNewSpaceShip_ScoutShip_NewScoutExpected()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9997");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu{name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(1);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        GameObject scout = Resources.Load("Object/Scout", typeof(GameObject)) as GameObject;
        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(2);
        GameObject newSpaceShip = Korriban.BuildSpaceship(scout);
        yield return new WaitForSeconds(3);

        Assert.AreEqual(newSpaceShip.name, "Scout(Clone)");
    }

    [UnityTest]
    public IEnumerator TestCreatingNewSpaceShip_WarShip_NewWarshipExpected()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9984");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(1);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        GameObject scout = Resources.Load("Object/Warship", typeof(GameObject)) as GameObject;
        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(2);
        GameObject newSpaceShip = Korriban.BuildSpaceship(scout);
        yield return new WaitForSeconds(3);

        Assert.AreEqual(newSpaceShip.name, "Warship(Clone)");
    }

    [UnityTest]
    public IEnumerator TestCreatingNewSpaceShip_MinerShip_NewMinerExpected()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9985");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(1);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        GameObject scout = Resources.Load("Object/Miner", typeof(GameObject)) as GameObject;
        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(2);
        GameObject newSpaceShip = Korriban.BuildSpaceship(scout);
        yield return new WaitForSeconds(3);

        Assert.AreEqual(newSpaceShip.name, "Miner(Clone)");
    }

    [UnityTest]
    public IEnumerator TestCreatingNewSpaceShip_Colonizer_NewColonizerExpected()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9986");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(1);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        GameObject scout = Resources.Load("Object/Colonizer", typeof(GameObject)) as GameObject;
        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(2);
        GameObject newSpaceShip = Korriban.BuildSpaceship(scout);
        yield return new WaitForSeconds(3);

        Assert.AreEqual(newSpaceShip.name, "Colonizer(Clone)");
    }

    [UnityTest]
    public IEnumerator TestColonizing_Player1_Player1OwnsMoon()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9998");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(1);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        GameObject colonizer = Resources.Load("Object/Colonizer", typeof(GameObject)) as GameObject;
        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        gameController.GetCurrentPlayer().minerals = 100;
        gameController.GetCurrentPlayer().population = 100;
        gameController.GetCurrentPlayer().solarPower = 100;
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(2);
        gameController.BuildSpaceship(colonizer);
        GameObject gameObjectcolonizerSpaceShip = GameObject.Find("Colonizer(Clone)");
        Colonizer colonizerSpaceShip = gameObjectcolonizerSpaceShip.GetComponent<Colonizer>();
        colonizerSpaceShip.actionPoints = 100;
        GameObject gameObjetMoon = GameObject.Find("Moon");
        Planet moon = gameObjetMoon.GetComponent<Planet>();
        colonizerSpaceShip.GetOwner().terraforming = 100;
        EventManager.selectionManager.SelectedObject = gameObjectcolonizerSpaceShip;
        yield return new WaitForSeconds(2);
        EventManager.selectionManager.TargetObject = gameObjetMoon;
        yield return new WaitForSeconds(2);
        gameController.Colonize();
        yield return new WaitForSeconds(2);

        Assert.AreEqual(moon.owner.name, "player1");
    }

    [UnityTest]
    public IEnumerator TestFire_2WarShips_2ShipsFighting()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(1);
        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(1);
        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9999");
        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(1);
        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu
        {
            name = "player1",
            password = "pass1",
            race = "human",
            playerType = "L"
        };
        GameApp.PlayerMenu player2 = new GameApp.PlayerMenu
        {
            name = "player12",
            password = "pass2",
            race = "human",
            playerType = "L"
        };
        playerMenuList.Add(player1);
        playerMenuList.Add(player2);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(1);
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        GameObject gameObjectWarShip = Resources.Load("Object/Warship", typeof(GameObject)) as GameObject;
        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        gameController.GetCurrentPlayer().minerals = 100;
        gameController.GetCurrentPlayer().population = 100;
        gameController.GetCurrentPlayer().solarPower = 100;
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(1);
        gameController.BuildSpaceship(gameObjectWarShip);
        yield return new WaitForSeconds(1);
        GameObject gameObjectWarshipSpaceShip1 = GameObject.Find("Warship(Clone)");
        Warship warShip1 = gameObjectWarshipSpaceShip1.GetComponent<Warship>();
        gameController.GetCurrentPlayer().engines = 100;
        warShip1.actionPoints = 100;
        gameController.GetCurrentPlayer().attack = 100;

        gameController.NextTurn();
        yield return new WaitForSeconds(1);
        gameController.GetCurrentPlayer().minerals = 100;
        gameController.GetCurrentPlayer().population = 100;
        gameController.GetCurrentPlayer().solarPower = 100;
        GameObject noVa = GameObject.Find("Nova");
        EventManager.selectionManager.SelectedObject = noVa;
        yield return new WaitForSeconds(1);
        gameController.BuildSpaceship(gameObjectWarShip);
        yield return new WaitForSeconds(1);
        GameObject gameObjectWarshipSpaceShip2 = GameObject.Find("Warship(Clone)");
        Warship warShip2 = gameObjectWarshipSpaceShip2.GetComponent<Warship>();
       
        gameController.NextTurn();
        yield return new WaitForSeconds(1);
        HexGrid grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        EventManager.selectionManager.SelectedObject = gameObjectWarshipSpaceShip1;
        yield return new WaitForSeconds(1);
        HexCoordinates des = new HexCoordinates(warShip2.Coordinates.X - 1, warShip2.Coordinates.Z);
        grid.TouchCell(des);
        yield return new WaitForSeconds(10);
        EventManager.selectionManager.TargetObject = gameObjectWarshipSpaceShip2;
        yield return new WaitForSeconds(1);
        gameController.Attack();
        yield return new WaitForSeconds(2);

        Assert.IsTrue(gameObjectWarshipSpaceShip2 == null);
    }
}
