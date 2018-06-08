using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Linq;

/**
 * Class represent War spaceship.
 */
public class Warship : Spaceship
{

    private new void Awake()
    {
        base.Awake();

        model = "Warship";
        MaxActionPoints = 5;
        RadarRange = 3;

        neededMinerals = gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededMinerals;
        neededPopulation = gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededPopulation;
        neededSolarPower = gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededSolarPower;

        spaceshipStatistics.healthPoints = 100;
        spaceshipStatistics.attack = 10;
        spaceshipStatistics.radars = 3;
        spaceshipStatistics.speed = 5;
    }


}