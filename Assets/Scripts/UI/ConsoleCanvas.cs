using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class ConsoleCanvas : MonoBehaviour
{

    bool Shown;

    private static bool created = false;
    private ConsolePanel panel;

    private void Awake()
    {
        if (!created)
        {
            //gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
            panel = GetComponentInChildren<ConsolePanel>();
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
    }

    void Start()
    {
        //panel.enabled = false;
        panel.gameObject.SetActive(false);
        Shown = true;
    }

    void Show()
    {
        //panel.enabled = true;
        panel.gameObject.SetActive(true);
        Shown = true;
    }

    void Hide()
    {
        //panel.enabled = false;
        panel.gameObject.SetActive(false);
        Shown = false;
    }

    public void Toggle()
    {
        if (Shown) Hide();
        else Show();
    }

    void Update()
    {

        if (Input.GetButtonUp("Console"))
        {
            Toggle();
        }
    }
}