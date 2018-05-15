﻿using Assets.Scripts;
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
    public Text Test;
    

    private void Awake()
    {
        MaxActionPoints =6;
        RadarRange = 15;
        buildCost = 10;
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
    public bool MineMinerals()
    {
        var gameObjectsInProximity =
                Physics.OverlapSphere(transform.position, 10)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();

        var cells = gameObjectsInProximity.Where(o => o.tag == "Planet");

        PlanetToMine = (cells.FirstOrDefault().GetComponent<Planet>() as Planet);
        if (PlanetToMine == null || PlanetToMine.GetOwner() == GameController.GetCurrentPlayer()) return false;
        else
        if (PlanetToMine.GiveMinerals(GameController.GetCurrentPlayer())) return true;
        return true;

    }
}