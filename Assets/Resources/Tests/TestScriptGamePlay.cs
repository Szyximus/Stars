using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.HexLogic;

public class TestScriptGamePlay : HexGrid
{

    [UnityTest]
    public IEnumerator TestNextTurn() {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        if (!SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
            Assert.Fail();

        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();

        if (initializer == null)
            Assert.Fail();

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        if (!SceneManager.GetActiveScene().name.Equals("NewGameMapScene"))
            Assert.Fail();


        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9999");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        if (!SceneManager.GetActiveScene().name.Equals("NewGameScene"))
            Assert.Fail();

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
        int number = 3;
        int previous = gameController.GetYear();
        for (int i = 0; i < number; i++)
        {
            gameController.NextTurn();
            int current = gameController.GetYear();
            yield return new WaitForSeconds(1);
            if (current != previous && current != previous + 1)
                Assert.Fail();
            previous = current;
        }
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMovementOfSpaceShip()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
            Assert.Fail();

        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();

        if (initializer == null)
            Assert.Fail();

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("NewGameMapScene"))
            Assert.Fail();


        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        if (gameApp == null)
            Assert.Fail();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9999");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        if (levelLoader == null)
            Assert.Fail();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("NewGameScene"))
            Assert.Fail();

        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        if (newGameSceneInit == null)
            Assert.Fail();
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
        if (gameController == null)
            Assert.Fail();

        GameObject gameObjectspaceShip = GameObject.Find("Scout-1");
        Spaceship spaceShip = gameObjectspaceShip.GetComponent<Spaceship>();
        if (spaceShip == null || gameObjectspaceShip == null)
            Assert.Fail();

        Debug.Log("1. I am here " + spaceShip.transform.localPosition);

        HexGrid grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        if (grid == null)
            Assert.Fail();
        Vector3 vector = new Vector3(310.0f, 0.0f, 270.0f);
        HexCoordinates destination = HexCoordinates.FromPosition(vector);
        spaceShip.Destination = destination;
        EventManager.selectionManager.SelectedObject = gameObjectspaceShip;
        yield return new WaitForSeconds(3);
        grid.TouchCell(vector);
        yield return new WaitForSeconds(3);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestCreatingNewSpaceShip()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
            Assert.Fail();

        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();

        if (initializer == null)
            Assert.Fail();

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("NewGameMapScene"))
            Assert.Fail();


        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        if (gameApp == null)
            Assert.Fail();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9999");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        if (levelLoader == null)
            Assert.Fail();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("NewGameScene"))
            Assert.Fail();

        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        if (newGameSceneInit == null)
            Assert.Fail();
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
        if (gameController == null)
            Assert.Fail();

        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        if (gameObjectKorriban == null)
            Assert.Fail();
        GameObject gameObjectspaceShip = GameObject.Find("Scout-1");
        if (gameObjectspaceShip == null)
            Assert.Fail();

        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(3);
        GameObject newSpaceShip = Korriban.BuildSpaceship(gameObjectspaceShip);
        yield return new WaitForSeconds(3);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestColonizing()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
            Assert.Fail();

        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();

        if (initializer == null)
            Assert.Fail();

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("NewGameMapScene"))
            Assert.Fail();


        GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        if (gameApp == null)
            Assert.Fail();
        gameApp.PersistAllParameters(SceneManager.GetActiveScene().name);
        gameApp.Parameters.Remove("ServerAddress");
        gameApp.Parameters.Remove("ServerPort");
        gameApp.Parameters.Add("ServerAddress", "127.0.0.1");
        gameApp.Parameters.Add("ServerPort", "9999");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        if (levelLoader == null)
            Assert.Fail();
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(1);
        if (!SceneManager.GetActiveScene().name.Equals("NewGameScene"))
            Assert.Fail();

        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        if (newGameSceneInit == null)
            Assert.Fail();
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
        if (gameController == null)
            Assert.Fail();

        GameObject gameObjectKorriban = GameObject.Find("Korriban");
        if (gameObjectKorriban == null)
            Assert.Fail();

        GameObject gameObjectspaceShip = Resources.Load("Object/Colonizer", typeof(GameObject)) as GameObject;
        if (gameObjectspaceShip == null)
            Assert.Fail();

        Planet Korriban = gameObjectKorriban.GetComponent<Planet>();
        gameController.GetCurrentPlayer().minerals = 100;
        gameController.GetCurrentPlayer().population = 100;
        gameController.GetCurrentPlayer().solarPower = 100;
        EventManager.selectionManager.SelectedObject = gameObjectKorriban;
        yield return new WaitForSeconds(2);
        gameController.BuildSpaceship(gameObjectspaceShip);

        GameObject gameObjectcolonizerSpaceShip = GameObject.Find("Colonizer(Clone)");
        Colonizer colonizerSpaceShip = gameObjectcolonizerSpaceShip.GetComponent<Colonizer>();
        if (colonizerSpaceShip == null)
            Assert.Fail();
        colonizerSpaceShip.actionPoints = 100;
        GameObject gameObjetMoon = GameObject.Find("Moon");
        Planet moon = gameObjetMoon.GetComponent<Planet>();
        if (moon == null)
            Assert.Fail();
        colonizerSpaceShip.GetOwner().terraforming = 100;
        EventManager.selectionManager.SelectedObject = gameObjectcolonizerSpaceShip;
        yield return new WaitForSeconds(2);
        EventManager.selectionManager.TargetObject = gameObjetMoon;
        yield return new WaitForSeconds(2);
        gameController.Colonize();
         yield return new WaitForSeconds(2);
        yield return null;
    }
}
