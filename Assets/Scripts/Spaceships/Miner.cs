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
    public Planet PlanetToColonize;
    public Text Test;

    private void Awake()
    {
        MaxActionPoints = 5;
        RadarRange = 25;
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
}