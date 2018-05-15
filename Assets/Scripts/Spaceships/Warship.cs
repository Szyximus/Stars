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
        neededMinerals = 5;
        neededPopulation = 5;
        neededSolarPower = 5;
        spaceshipStatistics.healthPoints = 300;
        spaceshipStatistics.attack = 10;
        spaceshipStatistics.defense = 50;
        spaceshipStatistics.speed = 5;
    }


}