using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class UpperPanel : MonoBehaviour
{
    Button menuButton;
    Button researchButton;
    public Text resourcesLabel;
    void Start()
    {
        researchButton = GameObject.Find("ResearchButton").GetComponent<Button>();
        menuButton = GameObject.Find("MenuButton").GetComponent<Button>();
        resourcesLabel = GameObject.Find("Resources").GetComponent<Text>();
    }

    private void Update()
    {
        Player currentPlayer = GameController.GetCurrentPlayer();
        if (currentPlayer != null)
        {
            resourcesLabel.text = "Minerals " + currentPlayer.minerals.ToString() + " Population " + currentPlayer.population.ToString()
                                  + " Energy " + currentPlayer.energy.ToString();
        }
        else
        {
            Debug.Log("Cannot find player");
        }
    }
}