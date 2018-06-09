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
 * Class represent colonizing spaceship.
 */
public class Colonizer : Spaceship
{
    public Planet PlanetToColonize;
    public Text Test;

    private new void Awake()
    {
        base.Awake();

        model = "Colonizer";
        MaxActionPoints = 3;
        RadarRange = 2;
        neededMinerals = gameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededMinerals;
        neededPopulation = gameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededPopulation;
        neededSolarPower = gameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededSolarPower;
        spaceshipStatistics.healthPoints = 150;


        spaceshipStatistics.attack = 0;
        spaceshipStatistics.radars = 2;
        spaceshipStatistics.speed = 3;
    }

    /**
     * The method checks if some of the planets are near the Colonizer and whether it is possible to colonize these planets.
     */
    public bool Colonize(Planet planetToColonize)
    {
        if (!CheckDistance(planetToColonize))
            return false;
        if (planetToColonize == null || planetToColonize.GetOwner() == this.GetOwner())
        {
            return false;
        }
        if (CheckCanBeColonizate(planetToColonize) && planetToColonize.GetOwner() == null && GetActionPoints() > 0)
        {
            GameObject SourceFire = Instantiate(gameApp.AttackPrefab, transform.position, transform.rotation);
            Destroy(SourceFire, 1f);
            planetToColonize.Colonize();
            Debug.Log("You colonized planet " + planetToColonize.name);
            return true;
        }
        else
        if (planetToColonize.GetOwner() != null && GetActionPoints() > 0)
        {
            if (CheckCanBeConquered(planetToColonize) && CheckCanBeColonizate(planetToColonize))
            {
                Debug.Log("You colonized " + planetToColonize.GetOwnerName() + "'s planet " + planetToColonize.name);
                planetToColonize.Colonize();
                return true;
            }
            Debug.Log("Planet's health points are over 0");

            return false;
        }
        return false;

    }
    private bool CheckCanBeColonizate(Planet planet)
    {
        return planet.characteristics.habitability <= GetOwner().terraforming;
    }
    private bool CheckCanBeConquered(Planet planet)
    {
        return (planet.characteristics.healthPoints <= 0);
    }

}
