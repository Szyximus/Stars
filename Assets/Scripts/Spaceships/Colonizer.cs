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
    private void Awake()
    {
        MaxActionPoints = 4;
        RadarRange = 20;
        neededMinerals = GameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededMinerals;
        neededPopulation = GameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededPopulation;
        neededSolarPower = GameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededSolarPower;
        spaceshipStatistics.healthPoints = 600;


        spaceshipStatistics.attack = 5;
        spaceshipStatistics.defense = 40;
        spaceshipStatistics.speed = 4;
    }

    /**
     * The method checks if some of the planets are near the Colonizer and whether it is possible to colonize these planets.
     */
    public bool Colonize(Planet planetToColonize)
    {
        if (!CheckDistance(planetToColonize))
            return false;

        if (planetToColonize == null || planetToColonize.GetOwner() == GameController.GetCurrentPlayer())
        {
            return false;
        }
        else
        if (CheckCanBeColonizate(planetToColonize) && planetToColonize.GetOwner() == null && GetActionPoints() > 0)
        {
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