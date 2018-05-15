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

    private void Awake()
    {
        MaxActionPoints = 4;
        RadarRange = 20;
        neededMinerals = 1000;
        neededPopulation = 100;
        neededSolarPower = 400;
        spaceshipStatistics.healtPoints = 600;
        spaceshipStatistics.attack = 5;
        spaceshipStatistics.defense = 40;
        spaceshipStatistics.speed = 4;
    }

    /**
     * The method checks if some of the planets are near the Colonizer and whether it is possible to colonize these planets.
     */
    public bool ColonizePlanet()
    {
        var gameObjectsInProximity =
                Physics.OverlapSphere(transform.position, 10)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();

        var cells = gameObjectsInProximity.Where(o => o.tag == "Planet");

        PlanetToColonize = (cells.FirstOrDefault().GetComponent<Planet>() as Planet);
        if (PlanetToColonize == null || PlanetToColonize.GetOwner() == GameController.GetCurrentPlayer()) return false;
        else
        if (CheckCanBeColonizate(PlanetToColonize) && PlanetToColonize.GetOwner() == null && GetActionPoints() > 0)
        {
            PlanetToColonize.Colonize();
            Debug.Log("You colonized planet " + PlanetToColonize.name);
            return true;
        }
        else
        if (PlanetToColonize.GetOwner() != null && GetActionPoints() > 0)
        {
            if (CheckCanBeConquered(PlanetToColonize) && CheckCanBeColonizate(PlanetToColonize))
            {
                Debug.Log("You colonized " + PlanetToColonize.GetOwnerName() + "'s planet " + PlanetToColonize.name);
                PlanetToColonize.Colonize();
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