using UnityEngine;
using System.Collections;

public class MainMenuSceneInit : MonoBehaviour
{


    private LevelLoader levelLoader;

    void Start()
    {
        Debug.Log("MainMenuSceneInit");

        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }

    public void ChangeScene(string scene)
    {
        levelLoader.LoadLevel(scene);
    }

    public void Quit()
    {
        levelLoader.Quit();
    }
}
