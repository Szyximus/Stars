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

public class TestLoadScene
{
    [UnityTest]
    public IEnumerator TestLoadSceneMainMenu() {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenuScene");
    }

    [UnityTest]
    public IEnumerator TestLoadSceneMainMap()
    {
        SceneManager.LoadScene("MainMenuScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "MainMenuScene");


        MainMenuSceneInit initializer = GameObject.Find("Initializer").GetComponent<MainMenuSceneInit>();
        Assert.IsNotNull(initializer);

        initializer.ChangeScene("NewGameMapScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameMapScene");
    }

    [UnityTest]
    public IEnumerator TestLoadSceneNewGameScene()
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
        gameApp.Parameters.Add("ServerPort", "9994");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        Assert.IsNotNull(levelLoader);
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameScene");
    }

    [UnityTest]
    public IEnumerator TestLoadGameInterface()
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
        gameApp.Parameters.Add("ServerPort", "9993");

        LevelLoader levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        Assert.IsNotNull(levelLoader);
        levelLoader.LoadLevel("NewGameScene");
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetActiveScene().name, "NewGameScene");

        NewGameSceneInit newGameSceneInit = GameObject.Find("Initializer").GetComponent<NewGameSceneInit>();
        Assert.IsNotNull(initializer);

        newGameSceneInit.AddPlayer();
        newGameSceneInit.AddPlayer();

        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        GameApp.PlayerMenu player1 = new GameApp.PlayerMenu {
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
        Assert.IsNotNull(serverNetworkManager);

        serverNetworkManager.SetupNewGame();
        yield return new WaitForSeconds(3);
    }

}
