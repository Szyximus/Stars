using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class UpperPanel : MonoBehaviour
{
    Text powerLabel;
    Text populationLabel;
    Text mineralsLabel;

    Text terraformingLabel;
    Text attackLabel;
    Text enginesLabel;
    Text radarsLabel;

    Text playerLabel;

    private GameController gameController;
    private bool initialized = false;

    public void Init()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        powerLabel = GameObject.Find("Power").GetComponentInChildren<Text>();
        populationLabel = GameObject.Find("Population").GetComponentInChildren<Text>();
        mineralsLabel = GameObject.Find("Resources").GetComponentInChildren<Text>();

        terraformingLabel = GameObject.Find("Terraforming").GetComponentInChildren<Text>();
        attackLabel = GameObject.Find("Attack").GetComponentInChildren<Text>();
        enginesLabel = GameObject.Find("Engines").GetComponentInChildren<Text>();
        radarsLabel = GameObject.Find("Radars").GetComponentInChildren<Text>();

        playerLabel = GameObject.Find("PlayerName").GetComponentInChildren<Text>();

        initialized = true;
    }

    private void Update()
    {
        if (initialized == false)
            return;

        Player currentPlayer = gameController.GetCurrentPlayer();
        if (currentPlayer == null)
            return;

        playerLabel.text = currentPlayer.ToString().Replace("(Player)", "");
        if (currentPlayer != null)
        {
            powerLabel.text = currentPlayer.solarPower.ToString();
            populationLabel.text = currentPlayer.population.ToString();
            mineralsLabel.text = currentPlayer.minerals.ToString();

            terraformingLabel.text = (currentPlayer.terraforming ).ToString();
            attackLabel.text = '+' + currentPlayer.attack.ToString();
            enginesLabel.text = '+' + currentPlayer.engines.ToString();
            radarsLabel.text = '+' + currentPlayer.radars.ToString();
        }
        else
        {
            Debug.Log("Cannot find player");
        }
    }
}