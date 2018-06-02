﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class MenuPanel : MonoBehaviour
{

    bool Shown;

    void Start()
    {
        gameObject.SetActive(false);
        Shown = false;
    }

    void Show()
    {
        gameObject.SetActive(true);
        Shown = true;
    }

    void Hide()
    {
        gameObject.SetActive(false);
        Shown = false;
    }

    public void Toggle()
    {
        if (Shown) Hide();
        else Show();
    }
}