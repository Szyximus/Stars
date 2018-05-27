using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameApp : MonoBehaviour
{

    private static bool created = false;

    // variables between scenes
    public Dictionary<string, string> Parameters;

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

    private string GetInputFiled(string inputFieldName)
    {
        if (GameObject.Find(inputFieldName) != null)
        {
            InputField savedGameInput = GameObject.Find(inputFieldName).GetComponent<InputField>();
            if (savedGameInput != null)
            {
                return savedGameInput.text;
            }
        }
        return null;
    }

    public void PersistInputField(string key, string inputFieldName)
    {
        string value = GetInputFiled(inputFieldName);
        if (value != null)
            Parameters.Add(key, value);
    }

    public void RemoveInputField(string key)
    {
        if (Parameters.ContainsKey(key))
            Parameters.Remove(key);
    }

}
