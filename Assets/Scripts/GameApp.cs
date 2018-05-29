using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameApp : MonoBehaviour
{
    public readonly string configsPath = "Assets/Configs/Resources/";
    private static bool created = false;

    // variables between scenes
    public Dictionary<string, string> Parameters;
    public Dictionary<string, List<string>> ParametersList;

    private Dictionary<string, List<ParameterMapping>> parametersToPersist = new Dictionary<string, List<ParameterMapping>>
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
                new ParameterMapping { name="PlayerIsLocal1", inputField = "MenuCanvas/PlayerLocal1Input" },
                new ParameterMapping { name="PlayerIsLocal2", inputField = "MenuCanvas/PlayerLocal2Input" },
                new ParameterMapping { name="PlayerIsLocal3", inputField = "MenuCanvas/PlayerLocal3Input" }
            }
        },
        {"JoinGameScene",  new List<ParameterMapping> {
                new ParameterMapping { name="ServerAddress", inputField = "MenuCanvas/ServerAddressInput" },
                new ParameterMapping { name="ServerPort", inputField = "MenuCanvas/ServerPortInput" },
                new ParameterMapping { name="PlayerName", inputField = "MenuCanvas/PlayerNameInput" },
                new ParameterMapping { name="Password", inputField = "MenuCanvas/PasswordInput" }
            }
        }
    };

    private struct ParameterMapping
    {
        public string name;
        public string inputField;
    }

    public struct PlayerMenu
    {
        public string name;
        public string password;
        public string local;
        public bool isHuman;
    }
    

    void Awake()
    {
        if (!created)
        {
            Parameters = new Dictionary<string, string>();

            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
    }

    public List<PlayerMenu> GetAllPlayersFromMenu()
    {
        List<PlayerMenu> playerMenuList = new List<PlayerMenu>();
        playerMenuList.Add(new PlayerMenu
        {
            name = GetAndRemoveInputField("PlayerName1"),
            password = GetAndRemoveInputField("PlayerPass1"),
            local = GetAndRemoveInputField("PlayerIsLocal1"),
            isHuman = true
        });
        playerMenuList.Add(new PlayerMenu
        {
            name = GetAndRemoveInputField("PlayerName2"),
            password = GetAndRemoveInputField("PlayerPass2"),
            local = GetAndRemoveInputField("PlayerIsLocal2"),
            isHuman = true
        });
        playerMenuList.Add(new PlayerMenu
        {
            name = GetAndRemoveInputField("PlayerName3"),
            password = GetAndRemoveInputField("PlayerPass3"),
            local = GetAndRemoveInputField("PlayerIsLocal3"),
            isHuman = true
        });
        return playerMenuList;
    }

    public void RemoveAllParameters()
    {
        parametersToPersist.Clear();
    }

    public void PersistAllParameters(string scene)
    {
        if (parametersToPersist.ContainsKey(scene))
        {
            foreach (ParameterMapping parameterMapping in parametersToPersist[scene])
            {
                PersistInputField(parameterMapping.name, parameterMapping.inputField);
            }
        }
    }

    private string FindInputFiled(string inputFieldName)
    {
        if (GameObject.Find(inputFieldName) != null)
        {
            InputField inputField = GameObject.Find(inputFieldName).GetComponent<InputField>();
            if (inputField != null)
            {
                return inputField.text;
            }

            Toggle toogle = GameObject.Find(inputFieldName).GetComponent<Toggle>();
            if (toogle != null)
            {
                return toogle ? "true" : "false";
            }
        }
        return null;
    }

    public void PersistInputField(string key, string inputFieldName)
    {
        string value = FindInputFiled(inputFieldName);
        if (value != null)
            Parameters.Add(key, value);
    }

    public void RemoveInputField(string key)
    {
        if (Parameters.ContainsKey(key))
            Parameters.Remove(key);
    }

    public string GetAndRemoveInputField(string key)
    {
        if (Parameters.ContainsKey(key))
        {
            string toReturn = Parameters[key];
            Parameters.Remove(key);
            return toReturn;
        }
        return null;
    }

}
