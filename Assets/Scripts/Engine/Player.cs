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
    public string race;
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
    [SyncVar]
    public bool looser;

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

    public struct ResearchStruct
    {
        public int attackNeededMinerals;
        public int attackNeededPopulation;
        public int attackNeededSolarPower;
        public int attackLevel;

        public int enginesNeededMinerals;
        public int enginesNeedesPopulation;
        public int enginesNeededSolarPower;
        public int enginesLevel;

        public int radarsNeededMinerals;
        public int radarsNeededPopulation;
        public int radarsNeededSolarPower;
        public int radarsLevel;

        public int terraformingNeededMinerals;
        public int terraformingNeededPopulation;
        public int terraformingNeededSolarPower;
        public int terraformingLevel;
    }

    public ResearchStruct researchStruct;

    // Use this for initialization
    void Awake()
    {
        System.Random random = new System.Random();
        spaceships = new ArrayList();
        planets = new ArrayList();
        minerals = 5;
        population = 5;
        solarPower = 5;
        terraforming = 1;
        attack = 0;
        engines = 0;
        radars = 0;
        looser = false;

        

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

        researchStruct.attackLevel = 0;
        researchStruct.attackNeededMinerals = 20;
        researchStruct.attackNeededPopulation = 10;
        researchStruct.attackNeededSolarPower = 12;

        researchStruct.enginesLevel = 0;
        researchStruct.enginesNeededMinerals = 10;
        researchStruct.enginesNeedesPopulation = 11;
        researchStruct.enginesNeededSolarPower = 20;

        researchStruct.terraformingLevel = 0;
        researchStruct.terraformingNeededMinerals = 12;
        researchStruct.terraformingNeededPopulation = 18;
        researchStruct.terraformingNeededSolarPower = 14;

        researchStruct.radarsLevel = 0;
        researchStruct.radarsNeededMinerals = 11;
        researchStruct.radarsNeededPopulation = 12;
        researchStruct.radarsNeededSolarPower = 15;
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
