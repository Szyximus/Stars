using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Linq;

/**
 * Class represent Scout spaceship.
 */
public class Scout : Spaceship
{

    private new void Awake()
    {
        base.Awake();

        model = "Scout";
        MaxActionPoints = 7;
        RadarRange = 3;

        neededMinerals = gameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededMinerals;
        neededPopulation = gameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededPopulation;
        neededSolarPower = gameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededSolarPower;

        spaceshipStatistics.healthPoints = 50;
        spaceshipStatistics.attack = 0;
        spaceshipStatistics.radars = 4;
        spaceshipStatistics.speed = 7;
    }


}