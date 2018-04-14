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

        PlanetToColonize = (cells.FirstOrDefault().GetComponent<Planet>() as Planet);
        //if (CheckCanBeColonizate(planetToColonize))
        //  {
        PlanetToColonize.Colonize();
        // }

    }
    private bool CheckCanBeColonizate(Planet planet)
    {
        return planet.Characteristics.Oxygen + planet.Characteristics.Radiation  + planet.Characteristics.Temperature < 100 ? true : false;
    }
}