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
    public IEnumerator TestNextTurn() {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenuScene");

        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        Assert.IsNotNull(initializer);

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameMapScene");

        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9995");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameScene");

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
        Assert.IsNotNull(gameController);
        Assert.AreEqual(gameController.currentPlayerIndex, 0);
        Assert.AreEqual(gameController.year, 2400);

        gameController.NextTurn();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(gameController.currentPlayerIndex, 1);
        Assert.AreEqual(gameController.year, 2400);

        gameController.NextTurn();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(gameController.currentPlayerIndex, 0);
        Assert.AreEqual(gameController.year, 2401);

        gameController.NextTurn();
        yield return new WaitForSeconds(1);
        Assert.AreEqual(gameController.currentPlayerIndex, 1);
        Assert.AreEqual(gameController.year, 2401);
    }

    [UnityTest]
    public IEnumerator TestMovementOfSpaceShip()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenuScene");

        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        Assert.IsNotNull(initializer);

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameMapScene");

        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9996");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameScene");

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
        Assert.IsNotNull(gameController);

        GameObject gameObjectspaceShip = GameObject.Find("Scout-1");
        Assert.IsNotNull(gameObjectspaceShip);
        Spaceship spaceShip = gameObjectspaceShip.GetComponent<Spaceship>();
        Assert.IsNotNull(spaceShip);

        HexGrid grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        Assert.IsNotNull(grid);

        Vector3 vector = new Vector3(310.0f, 0.0f, 270.0f);
        HexCoordinates destination = HexCoordinates.FromPosition(vector);
        spaceShip.Destination = destination;
        EventManager.selectionManager.SelectedObject = gameObjectspaceShip;
        yield return new WaitForSeconds(2);
        grid.TouchCell(vector);
        yield return new WaitForSeconds(3);
    }

    [UnityTest]
    public IEnumerator TestCreatingNewSpaceShip()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenuScene");

        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        Assert.IsNotNull(initializer);

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameMapScene");

        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9997");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameScene");

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
        Assert.IsNotNull(gameController);

        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        Assert.IsNotNull(gameObjectKorriban);

        GameObject gameObjectspaceShip = GameObject.Find("Scout-1");
        Assert.IsNotNull(gameObjectspaceShip);

        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(2);
        GameObject newSpaceShip = Korriban.BuildSpaceship(gameObjectspaceShip);
        yield return new WaitForSeconds(3);
    }

    [UnityTest]
    public IEnumerator TestColonizing()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenuScene");

        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        Assert.IsNotNull(initializer);

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameMapScene");

        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9998");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameScene");

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
        Assert.IsNotNull(gameController);

        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        Assert.IsNotNull(gameObjectKorriban);

        GameObject gameObjectspaceShip = Resources.Load("Object/Colonizer", typeof(GameObject)) as GameObject;
        Assert.IsNotNull(gameObjectspaceShip);

        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        gameController.GetCurrentPlayer().minerals = 100;
        gameController.GetCurrentPlayer().population = 100;
        gameController.GetCurrentPlayer().solarPower = 100;
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(2);
        gameController.BuildSpaceship(gameObjectspaceShip);

        GameObject gameObjectcolonizerSpaceShip = GameObject.Find("Colonizer(Clone)");
        Assert.IsNotNull(gameObjectcolonizerSpaceShip);

        Colonizer colonizerSpaceShip = gameObjectcolonizerSpaceShip.GetComponent<Colonizer>();
        Assert.IsNotNull(colonizerSpaceShip);

        colonizerSpaceShip.actionPoints = 100;
        GameObject gameObjetMoon = GameObject.Find("Moon");
        Assert.IsNotNull(gameObjetMoon);

        Planet moon = gameObjetMoon.GetComponent<Planet>();
        Assert.IsNotNull(moon);

        colonizerSpaceShip.GetOwner().terraforming = 100;
        EventManager.selectionManager.SelectedObject = gameObjectcolonizerSpaceShip;
        yield return new WaitForSeconds(2);
        EventManager.selectionManager.TargetObject = gameObjetMoon;
        yield return new WaitForSeconds(2);
        gameController.Colonize();
        yield return new WaitForSeconds(2);
    }
}
