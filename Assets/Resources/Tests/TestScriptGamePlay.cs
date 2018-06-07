using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using Assets.Scripts;
using System.Collections.Generic;

public class TestScriptGamePlay {

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
 
}
