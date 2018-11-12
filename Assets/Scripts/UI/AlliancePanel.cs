using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;

public class AlliancePanel : MonoBehaviour
{

    Button yesButton;
    Button noButton;
    GameController gameController;

    bool shown;
    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
        shown = false;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
