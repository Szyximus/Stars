using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {

    /// <summary>
    /// Loads the DemoScene
    /// </summary>

    public GameObject slider;
    private GameApp gameApp;

    private void Awake()
    {
        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
    }

    public void Back(string scene)
    {
        gameApp.RemoveAllParameters();

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
        gameApp.PersistAllParameters(scene);

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
