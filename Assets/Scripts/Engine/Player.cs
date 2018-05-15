using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public bool human;
    public int minerals;
    public int population;
    public int energy;
    private ArrayList spaceships;
    private ArrayList planets;

    // Use this for initialization
    void Awake()
    {
        System.Random random = new System.Random();
        spaceships = new ArrayList();
        planets = new ArrayList();
        minerals = random.Next(1, 15);
        population = random.Next(1, 15);
        energy = random.Next(1, 15);
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
    public void AddMinerals(int minerals)
    {
        this.minerals += minerals;
    }
    public void AddPopulation(int population)
    {
        this.population += population;
    }
    public void AddEnergy(int energy)
    {
        this.energy += energy;
    }
}
