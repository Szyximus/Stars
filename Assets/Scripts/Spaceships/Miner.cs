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
        MaxActionPoints = 4;
        RadarRange = 2;

        neededMinerals = gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededMinerals;
        neededPopulation = gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededPopulation;
        neededSolarPower = gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededSolarPower;
        spaceshipStatistics.healthPoints = 75;

        spaceshipStatistics.attack = 0;
        spaceshipStatistics.radars = 2;
        spaceshipStatistics.speed = 4;
    }

    private bool CheckCanBeMined(Star star)

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
    public void MinePlanet(Planet planetToMine)
    {
        var miner = EventManager.selectionManager.SelectedObject.GetComponent<Miner>();
        if (miner != null && miner.GetActionPoints() > 0)
        {
            if (EventManager.selectionManager.TargetObject != null &&
                EventManager.selectionManager.TargetObject.GetComponent<Planet>() != null)
                if (miner.CheckCanBeMined(planetToMine))
                {
                    GameObject SourceFire = Instantiate(gameApp.AttackPrefab, transform.position, transform.rotation);
                    Destroy(SourceFire, 1f);
                    planetToMine.GiveMineralsTo(GetOwner(), 1);
                }
                else
                {
                    Debug.Log("Cannot mine");
                }
        }
    }
    public void MineStar(Star startToMine)
    {
        var miner = EventManager.selectionManager.SelectedObject.GetComponent<Miner>();
        if (miner != null && miner.GetActionPoints() > 0)
            if (EventManager.selectionManager.TargetObject != null &&
                EventManager.selectionManager.TargetObject.GetComponent<Star>() != null)
                if (miner.CheckCanBeMined(startToMine))
                {
                    GameObject SourceFire = Instantiate(gameApp.AttackPrefab, transform.position, transform.rotation);
                    Destroy(SourceFire, 1f);
                    startToMine.GiveSolarPower(GetOwner(), 1);
                }
                else
                {
                    Debug.Log("Cannot mine");
                }
    }

}