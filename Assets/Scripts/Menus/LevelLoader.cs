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

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/*
 *  Class for pretty scene changing, menu navigation buttons
 *  Singleton, created at "MainMenuScene", should be available all the time
 *  
 *  It destroys networkManagers on "Back"
 */
public class LevelLoader : MonoBehaviour {

    public GameObject slider;
    private GameApp gameApp;

    private static LevelLoader instance;

    private void Awake()
    {
        if (instance == null)
        {
            gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();

            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Back(string scene)
    {

        GameObject serverNetworkManager = GameObject.Find("ServerNetworkManager");
        if (serverNetworkManager != null)
            Destroy(serverNetworkManager);

        GameObject clientNetworkManager = GameObject.Find("ClientNetworkManager");
        if (clientNetworkManager != null)
            Destroy(clientNetworkManager);

        if (slider != null)
            slider.SetActive(true);

        StartCoroutine(LoadAsynchronously(scene));
    }

    public void LoadLevel (string scene)
    {
        if (slider != null)
            slider.SetActive(true);

        StartCoroutine(LoadAsynchronously(scene));
    }

    IEnumerator LoadAsynchronously(string scene)
    { 
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (slider != null) slider.GetComponent<Slider>().value = progress;
            yield return null;
        } 
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
}
