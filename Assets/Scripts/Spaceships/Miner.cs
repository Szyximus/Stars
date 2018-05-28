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
        MaxActionPoints = 6;
        RadarRange = 15;

        neededMinerals = GameController.GetCurrentPlayer().spaceshipsCosts.minerNeededMinerals;
        neededPopulation = GameController.GetCurrentPlayer().spaceshipsCosts.minerNeededPopulation;
        neededSolarPower = GameController.GetCurrentPlayer().spaceshipsCosts.minerNeededSolarPower;
        spaceshipStatistics.healthPoints = 80;

        spaceshipStatistics.attack = 5;
        spaceshipStatistics.defense = 15;
        spaceshipStatistics.speed = 6;
    }

    public bool MinePlanet()
    {
        return true;
    }

    public bool MineStar()
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
    public bool MineResources()
    {
        var gameObjectsInProximity =
                Physics.OverlapSphere(transform.position, 10)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();

        var planets = gameObjectsInProximity.Where(o => o.tag == "Planet");
        var stars = gameObjectsInProximity.Where(o => o.tag == "Star");

        try
        {
            PlanetToMine = planets.FirstOrDefault().GetComponent<Planet>() as Planet;
            Debug.Log("Jest planeta");
        }
        catch (System.Exception e)
        {
            Debug.Log("Nie ma planety");
        }
        try
        {
            StarToMine = stars.FirstOrDefault().GetComponent<Star>() as Star;
            Debug.Log("Jest gwiazda");
        }
        catch (System.Exception e)
        {
            Debug.Log("Nie ma gwiazdy");
        }
        if (StarToMine == null )
        {
            Debug.Log("Cannot find star/planet or planet belong to you");
            return false;
        }
        else if (StarToMine != null)
        {
            if (GetActionPoints() > 0)
            {
                StarToMine.GiveSolarPower(GetOwner(), 1);
                return true;
            }
            Debug.Log("You dont have enough movement points");
            return false;
        }
        else if (PlanetToMine != null || PlanetToMine.GetOwner() != GameController.GetCurrentPlayer())
        {
            if (GetActionPoints() > 0)
            {
                PlanetToMine.GiveMineralsTo(GetOwner(),1);
                return true;
            }
            Debug.Log("You dont have enough movement points");
            return false;
        }
        return false;

    }
}