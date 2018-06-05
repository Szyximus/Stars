using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class ConsoleCanvas : MonoBehaviour
{

    bool Shown;

    private static ConsoleCanvas instance;
    private ConsolePanel panel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            panel = GetComponentInChildren<ConsolePanel>();
            panel.gameObject.SetActive(false);
            Shown = false;
        } else if(instance != this)
        {
            Destroy(gameObject);
        }
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