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
public class Miner : Spaceship
{
    public Planet PlanetToColonize;
    public Text Test;

    private void Awake()
    {
        MaxActionPoints = 5;
        RadarRange = 25;
    }

    /**
     * The method checks if some of the planets are near the Colonizer and whether it is possible to colonize these planets.
     */
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
}