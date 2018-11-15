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

using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Linq;

/**
 * Class represent Mining spaceship.
 */
public class Miner : Spaceship
{
    public Planet PlanetToMine;
    public Star StarToMine;
    public Text Test;

    private new void Awake()
    {
        base.Awake();

        model = "Miner";
        MaxActionPoints = 4;
        RadarRange = 2;

        neededMinerals = gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededMinerals;
        neededPopulation = gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededPopulation;
        neededSolarPower = gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededSolarPower;
        spaceshipStatistics.healthPoints = 75;

        spaceshipStatistics.attack = 0;
        spaceshipStatistics.radars = 2;
        spaceshipStatistics.speed = 4;
    }

    private bool CheckCanBeMined(Star star)

    {
        return true;
    }
    private bool CheckCanBeMined(Planet planet)
    {
        return true;
    }

    /**
     * The method checks if some of the planets are near the Colonizer and whether it is possible to colonize these planets.
     */
    public void MinePlanet(Planet planetToMine)
    {
        var miner = EventManager.selectionManager.SelectedObject.GetComponent<Miner>();
        if (miner != null && miner.GetActionPoints() > 0)
        {
            if (EventManager.selectionManager.TargetObject != null &&
                EventManager.selectionManager.TargetObject.GetComponent<Planet>() != null)
                if (miner.CheckCanBeMined(planetToMine))
                {
                    GameObject SourceFire = Instantiate(gameApp.MinePrefab, transform.position, transform.rotation);
                    Destroy(SourceFire, 1f);
                    planetToMine.GiveMineralsTo(GetOwner(), 1);
                }
                else
                {
                    Debug.Log("Cannot mine");
                }
        }
    }
    public void MineStar(Star startToMine)
    {
        var miner = EventManager.selectionManager.SelectedObject.GetComponent<Miner>();
        if (miner != null && miner.GetActionPoints() > 0)
            if (EventManager.selectionManager.TargetObject != null &&
                EventManager.selectionManager.TargetObject.GetComponent<Star>() != null)
                if (miner.CheckCanBeMined(startToMine))
                {
                    GameObject SourceFire = Instantiate(gameApp.MinePrefab, transform.position, transform.rotation);
                    Destroy(SourceFire, 1f);
                    startToMine.GiveSolarPower(GetOwner(), 1);
                }
                else
                {
                    Debug.Log("Cannot mine");
                }
    }

}
