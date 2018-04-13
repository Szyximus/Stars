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
    GameObject Object;
    public Planet planetToColonize;
    public Text Test;

    private Player owner;

    /**
     * The method checks if some of the planets are near the Colonizer and whether it is possible to colonize these planets.
     */
    public void ColonizePlanet()
    {
        var colonizer = FindObjectOfType<Colonizer>();
        var gameObjectsInProximity =
                Physics.OverlapSphere(colonizer.transform.position, 10)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();

        var cells = gameObjectsInProximity.Where(o => o.tag == "Planet");

        planetToColonize = (cells.FirstOrDefault().GetComponent<Planet>() as Planet);
        //if (CheckCanBeColonizate(planetToColonize))
        //  {
        planetToColonize.ColonizePlanet(owner);
        // }

    }
    private bool CheckCanBeColonizate(Planet planet)
    {
        return planet.characteristics.oxygen + planet.characteristics.radiation  + planet.characteristics.temperature < 100 ? true : false;
    }
}