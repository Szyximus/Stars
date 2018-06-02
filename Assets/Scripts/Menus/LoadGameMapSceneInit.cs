using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class LoadGameMapSceneInit : MonoBehaviour
{

    private GameApp gameApp;
    private LevelLoader levelLoader;

    void Start()
    {
        Debug.Log("LoadGameMapSceneInit");

        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

        Dropdown mapToLoadDropdown = GameObject.Find("MenuCanvas/DropdownCanvas/GameToLoadDropdown").GetComponent<Dropdown>();
        string[] foundFiles = Directory.GetFiles(gameApp.savedGamesPath, "*.json");
        foreach (string foundFile in foundFiles)
        {
            mapToLoadDropdown.options.Add(new Dropdown.OptionData() { text = Path.GetFileNameWithoutExtension(foundFile) });
        }

    }

    public void Back()
    {
        levelLoader.Back("mainMenuScene");
    }

    public void Next()
    {
        gameApp.PersistAllParameters("LoadGameMapScene");
        levelLoader.LoadLevel("LoadGameScene");
    }
}
