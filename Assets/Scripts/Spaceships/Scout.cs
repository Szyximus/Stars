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

    private void Awake()
    {
        MaxActionPoints = 7;
        RadarRange = 25;
        neededMinerals = 10;
        neededPopulation = 8;
        neededSolarPower = 8;
        spaceshipStatistics.healthPoints = 150;
        spaceshipStatistics.attack = 10;
        spaceshipStatistics.defense = 25;
        spaceshipStatistics.speed = 7;
    }


}