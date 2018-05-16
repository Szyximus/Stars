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

    private void Awake()
    {
        MaxActionPoints = 5;
        RadarRange = 25;

        neededMinerals = GameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededMinerals;
        neededPopulation = GameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededPopulation;
        neededSolarPower = GameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededSolarPower;

        spaceshipStatistics.healthPoints = 300;
        spaceshipStatistics.attack = 35;
        spaceshipStatistics.defense = 50;
        spaceshipStatistics.speed = 5;
    }


}