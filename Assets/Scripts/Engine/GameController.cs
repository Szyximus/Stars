using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    private Player[] players;
    public Player playerPrefab;
    private int currentPlayerIndex;

    public Planet planetPrefab;
    public Star startPrefab;
    private Star[] stars;

    // Use this for initialization
    void Start()
    {
        initPlayers();
        initMap();
    }

    void initPlayers()
    {
        // Create players from prefab.
        // todo: should be done after main menu
        players = new Player[3];
        players[0] = Instantiate(playerPrefab);
        players[0].human = true;
        players[0].name = "Main Player";

        for (int i = 0; i < 2; i++)
        {
            players[i] = Instantiate(playerPrefab);
            players[i].human = false;
            players[i].name = "AI-" + i;
        }

        currentPlayerIndex = 0;
    }

    void initMap()
    {
        // Create map from file / random.
        // todo: in main menu we should decide if map is from file or random and set parameters
        // serializacje w unity ssie, trzeba bedzie doprawcowac (potrzebne bedzie do save/load i pewnie networkingu...)
        // todo: w jsonach nie moze byc utf8

        PlanetsCollection planetsCollection = JsonUtility.FromJson<PlanetsCollection>(Resources.Load("map1").ToString()) as PlanetsCollection;
        foreach (PlanetsCollection.PlanetSerialized planetSerialized in planetsCollection.planets)
        {
            Planet planet = Instantiate(original: planetPrefab, position: new Vector3(planetSerialized.position[0], planetSerialized.position[1], planetSerialized.position[2]),
                rotation: Quaternion.identity);
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(planetSerialized.planetMain), planet);
            planet.name = planetSerialized.name;
            planet.GetComponent<SphereCollider>().radius = planetSerialized.radius;
        }
    }

    void nextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
    }

    Player getCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
