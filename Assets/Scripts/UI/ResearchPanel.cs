using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class ResearchPanel : MonoBehaviour
{
    Text TerraformingCosts;
    Text AttackCosts;
    Text EnginesCosts;
    Text RadarsCosts;
    GameController gameController;

    bool Shown;

    void Start()
    {


        TerraformingCosts = GetComponentsInChildren<Text>()[1];
        AttackCosts = GetComponentsInChildren<Text>()[2];
        EnginesCosts = GetComponentsInChildren<Text>()[3];
        RadarsCosts = GetComponentsInChildren<Text>()[4];

        gameObject.SetActive(false);
        Shown = false;
    }

    void Show()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        gameObject.SetActive(true);
        Shown = true;

        if (gameController.GetCurrentPlayer() != null) {
            TerraformingCosts.text = "-" + gameController.GetCurrentPlayer().researchStruct.terraformingNeededSolarPower.ToString() +
                                    "   " + "-" + gameController.GetCurrentPlayer().researchStruct.terraformingNeededMinerals.ToString() +
                                    "   " + "-" + gameController.GetCurrentPlayer().researchStruct.terraformingNeededPopulation.ToString();
            AttackCosts.text = "-" + gameController.GetCurrentPlayer().researchStruct.attackNeededSolarPower.ToString() +
                                        "   " + "-" + gameController.GetCurrentPlayer().researchStruct.attackNeededMinerals.ToString() +
                                        "   " + "-" + gameController.GetCurrentPlayer().researchStruct.attackNeededPopulation.ToString();
            EnginesCosts.text = "-" + gameController.GetCurrentPlayer().researchStruct.enginesNeededSolarPower.ToString() +
                                        "   " + "-" + gameController.GetCurrentPlayer().researchStruct.enginesNeededMinerals.ToString() +
                                        "   " + "-" + gameController.GetCurrentPlayer().researchStruct.enginesNeedesPopulation.ToString();
            RadarsCosts.text = "-" + gameController.GetCurrentPlayer().researchStruct.radarsNeededSolarPower.ToString() +
                                        "   " + "-" + gameController.GetCurrentPlayer().researchStruct.radarsNeededMinerals.ToString() +
                                        "   " + "-" + gameController.GetCurrentPlayer().researchStruct.radarsNeededPopulation.ToString();
        }

        
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