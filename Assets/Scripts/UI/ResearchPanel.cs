/*
    Copyright (c) 2018, Szymon Jakóbczyk, Paweł Płatek, Michał Mielus, Maciej Rajs, Minh Nhật Trịnh, Izabela Musztyfaga
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation 
          and/or other materials provided with the distribution.
        * Neither the name of the [organization] nor the names of its contributors may be used to endorse or promote products derived from this software 
          without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
    USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

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
