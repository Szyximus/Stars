using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/*
 *  Class for global configuration and persistance data between scenes
 *  Singleton, created at "MainMenuScene", should be available all the time
 *  
 *  When some scene with input fields will be changed soon, this class can persists data from the scene
 */
public class GameApp : MonoBehaviour
{
    private static bool created = false;

    // base path for saved games files and new game files
    public string configsPath;

    // ids for network messaging
    public static readonly short connMapJsonId = 1337;
    public static readonly short connAssignPlayerId = 20001;
    public static readonly short connAssignPlayerErrorId = 20002;
    public static readonly short connAssignPlayerSuccessId = 20003;
    public static readonly short connClientReadyId = 20004;

    // variables that will be available between scenes
    public Dictionary<string, string> Parameters;

    // data that will be saved, based on scene name
    // Dictionary<scene name, List<{saved value name, path to input filed in editor}>>
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
        {"LoadGameScene",  new List<ParameterMapping> {
                new ParameterMapping { name="Address", inputField = "MenuCanvas/AddressInput" },
                new ParameterMapping { name="Port", inputField = "MenuCanvas/PortInput" },
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

    // Scripts can receive inputs by "name". Inputs are found by object name ("inputField") in editor 
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

            configsPath = Application.persistentDataPath + "/Configs/";
            Debug.Log("GameApp configsPath: " + configsPath);

            DontDestroyOnLoad(this.gameObject);
            created = true;

            Debug.Log("Awake: " + this.gameObject);
        }
    }

    /*
     *  Persists players data from "NewGameScene". todo: change to list
     */
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
        Parameters.Clear();
    }

    /*
     *  Get current scene name and persists all input fields
     */
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
