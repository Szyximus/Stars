using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class ConsolePanel : MonoBehaviour
{
    //#if !UNITY_EDITOR
    static string myLog = "";
    private string output;
    private string stack;


    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
    }

    void OnGUI()
    {
        {
            myLog = GUI.TextArea(new Rect(25, 25, 512 - 25, 512 - 25), myLog);
        }
    }
}