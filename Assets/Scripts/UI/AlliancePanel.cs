using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;
using System;

public class AlliancePanel : MonoBehaviour
{
    public bool makeAlliance = false;

    Text playerText;
    Text allianceText;
    ArrayList allianceList;
    public bool buttonClicked = false;

    private GameController gameController;


    public void Init()
    {
        gameObject.SetActive(false);
        playerText = GetComponentsInChildren<Text>()[0];
        allianceText = GetComponentsInChildren<Text>()[1];

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
    // Use this for initialization
    void Start()
    {

    }

    public void ActionAfterClickYes()
    {
        makeAlliance = true;
        buttonClicked = true;
    }

    public void ActionAfterClickNo()
    {
        makeAlliance = false;
        buttonClicked = true;
    }
    public void SetPlayer(Player player)
    {
        playerText.color = player.color;
        playerText.text = player.name;
        allianceText.text = " wants to forge an alliance. What's your answer?";
    }
    // Update is called once per frame
    void Update()
    {

        if (allianceList != null && allianceList.Count != 0)
        {
            Player player = (Player)allianceList[0];

            SetPlayer(player);
            if (buttonClicked == true)
            {
                if (makeAlliance == true)
                {
                    gameController.GetCurrentPlayer().AddToAllies(player);
                    player.AddToAllies(gameController.GetCurrentPlayer());
                    gameController.GetCurrentPlayer().playersAskingAboutAlliance.Remove(player);
                    gameObject.SetActive(false);
                }
                else
                {
                    gameController.GetCurrentPlayer().playersAskingAboutAlliance.Remove(player);
                    gameObject.SetActive(false);
                }
                buttonClicked = false;
                gameController.showAllianceButton = true;
            }
        }
    }



    //do poprawy
    internal void ManageAliance(ArrayList arrayList)
    {
        gameObject.SetActive(true);
        allianceList = arrayList;

    }

}
