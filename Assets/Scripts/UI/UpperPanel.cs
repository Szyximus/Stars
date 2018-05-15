using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class UpperPanel : MonoBehaviour
{
    public Text powerLabel;
    public Text populationLabel;
    public Text mineralsLabel;
    void Start()
    {
        powerLabel = GameObject.Find("Power").GetComponentInChildren<Text>();
        populationLabel = GameObject.Find("Population").GetComponentInChildren<Text>();
        mineralsLabel = GameObject.Find("Resources").GetComponentInChildren<Text>();
    }

    private void Update()
    {
        Player currentPlayer = GameController.GetCurrentPlayer();
        if (currentPlayer != null)
        {
            powerLabel.text = currentPlayer.solarPower.ToString();
            populationLabel.text = currentPlayer.population.ToString();
            mineralsLabel.text = currentPlayer.minerals.ToString();
        }
        else
        {
            Debug.Log("Cannot find player");
        }
    }
}