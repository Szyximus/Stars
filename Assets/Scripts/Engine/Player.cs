using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[System.Serializable]
public class Player : NetworkBehaviour
{
    public string password;
    public bool local;
    public bool clientConnected;

    [SyncVar]
    public bool human;
    [SyncVar]
    public int minerals;
    [SyncVar]
    public int population;
    [SyncVar]
    public int solarPower;
    [SyncVar]
    public int terraforming;
    [SyncVar]
    public int attack;
    [SyncVar]
    public int engines;
    [SyncVar]
    public int radars;

    private ArrayList spaceships;
    private ArrayList planets;

    [System.Serializable]
    public struct SpaceshipsCosts
    {
        public int scoutNeededMinerals;
        public int scoutNeededPopulation;
        public int scoutNeededSolarPower;

        public int minerNeededMinerals;
        public int minerNeededPopulation;
        public int minerNeededSolarPower;

        public int warshipNeededMinerals;
        public int warshipNeededPopulation;
        public int warshipNeededSolarPower;

        public int colonizerNeededMinerals;
        public int colonizerNeededPopulation;
        public int colonizerNeededSolarPower;
    }

    [SyncVar]
    public SpaceshipsCosts spaceshipsCosts;

    // Use this for initialization
    void Awake()
    {
        System.Random random = new System.Random();
        spaceships = new ArrayList();
        planets = new ArrayList();
        minerals = 10;
        population = 10;
        solarPower = 10;
        terraforming = 1;
        attack = 0;
        engines = 0;
        radars = 0;

        spaceshipsCosts.scoutNeededMinerals = 5;
        spaceshipsCosts.scoutNeededPopulation = 2;
        spaceshipsCosts.scoutNeededSolarPower = 4;

        spaceshipsCosts.minerNeededMinerals = 2;
        spaceshipsCosts.minerNeededPopulation = 5;
        spaceshipsCosts.minerNeededSolarPower = 6;

        spaceshipsCosts.warshipNeededMinerals = 6;
        spaceshipsCosts.warshipNeededPopulation = 4;
        spaceshipsCosts.warshipNeededSolarPower = 5;

        spaceshipsCosts.colonizerNeededMinerals = 7;
        spaceshipsCosts.colonizerNeededPopulation = 9;
        spaceshipsCosts.colonizerNeededSolarPower = 8;


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
