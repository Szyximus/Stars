﻿using System.Collections;
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
