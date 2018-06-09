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
