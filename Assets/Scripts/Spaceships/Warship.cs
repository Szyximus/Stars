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
        RadarRange = 25;

        neededMinerals = gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededMinerals;
        neededPopulation = gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededPopulation;
        neededSolarPower = gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededSolarPower;

        spaceshipStatistics.healthPoints = 300;
        spaceshipStatistics.attack = 35;
        spaceshipStatistics.defense = 50;
        spaceshipStatistics.speed = 5;
    }


}