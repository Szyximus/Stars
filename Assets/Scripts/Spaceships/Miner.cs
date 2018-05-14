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
    public Planet PlanetToMine;
    public Text Test;

    private void Awake()
    {
        MaxActionPoints = 6;
        RadarRange = 15;
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
        if (PlanetToMine.Colonize()) return true;
        return true;

    }
}