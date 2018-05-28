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

    public Dictionary<string, List<ParameterMapping>> parametersToPersist = new Dictionary<string, List<ParameterMapping>>
    {
        {"GameScene",  new List<ParameterMapping> {
                new ParameterMapping { name="SavedGameFile", inputField = "MenuCanvas/SavedGameFileInput" }
            }
        },
        {"NewGameScene",  new List<ParameterMapping> {
                new ParameterMapping { name="Address", inputField = "MenuCanvas/AddressInput" },
                new ParameterMapping { name="Port", inputField = "MenuCanvas/PortInput" },
                new ParameterMapping { name="PlayerName1", inputField = "MenuCanvas/PlayerName1Input" },
                new ParameterMapping { name="PlayerName2", inputField = "MenuCanvas/PlayerName2Input" },
                new ParameterMapping { name="PlayerName3", inputField = "MenuCanvas/PlayerName3Input" },
                new ParameterMapping { name="PlayerPass1", inputField = "MenuCanvas/PlayerPass1Input" },
                new ParameterMapping { name="PlayerPass2", inputField = "MenuCanvas/PlayerPass2Input" },
                new ParameterMapping { name="PlayerPass3", inputField = "MenuCanvas/PlayerPass3Input" },
            }
        },
        {"JoinGameScene",  new List<ParameterMapping> {
                new ParameterMapping { name="ServerAddress", inputField = "MenuCanvas/ServerAddressInput" },
                new ParameterMapping { name="ServerPort", inputField = "MenuCanvas/ServerPortInput" },
                new ParameterMapping { name="PlayerName", inputField = "MenuCanvas/PlayerNameInput" },
                new ParameterMapping { name="Password", inputField = "MenuCanvas/PasswordInput" },
            }
        }
    };

    public struct ParameterMapping
    {
        public string name;
        public string inputField;
    }
    

    public void LoadLevel (string scene)
    {
        if (slider != null)
            slider.SetActive(true);
        StartCoroutine(LoadAsynchronously(scene));
    }

    IEnumerator LoadAsynchronously(string scene)
    {
        if (parametersToPersist.ContainsKey(scene)) {
            GameApp gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
            if (gameApp != null)
            {
                foreach (ParameterMapping parameterMapping in parametersToPersist[scene])
                {
                    gameApp.PersistInputField(parameterMapping.name, parameterMapping.inputField);
                }
            } else
            {
                Debug.Log("gameApp is null");
            }
        }

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
