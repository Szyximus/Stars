using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public bool human;
    public int minerals;
    public int population;
    public int solarPower;
    public int terraforming;
    private ArrayList spaceships;
    private ArrayList planets;

    // Use this for initialization
    void Awake()
    {
        System.Random random = new System.Random();
        spaceships = new ArrayList();
        planets = new ArrayList();
        minerals = 1000;
        population = 50;
        solarPower = 60;
        terraforming = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerable GetOwned()
    {
        foreach (Planet planet in planets)
        {
            yield return planet;
        }
        foreach (Spaceship spaceship in spaceships)
        {
            yield return spaceship;
        }
    }

    public IEnumerable GetSpaceships()
    {
        foreach (Spaceship spaceship in spaceships)
        {
            yield return spaceship;
        }
    }

    public IEnumerable GetPlanets()
    {
        foreach (Planet planet in planets)
        {
            yield return planet;
        }
    }

    public void Own(Ownable thing)
    {
        if (thing is Planet)
            planets.Add(thing);
        if (thing is Spaceship)
            spaceships.Add(thing);
    }

    public void Lose(Ownable thing)
    {
        if (thing is Planet)
            planets.Remove(thing);
        if (thing is Spaceship)
            spaceships.Remove(thing);
    }
}
