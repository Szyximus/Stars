using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Assets.Scripts;

public class GameController : MonoBehaviour
{
    private static GameObject[] players;
    public GameObject playerPrefab;
    private static int currentPlayerIndex;

    public GameObject planetPrefab;
    public GameObject startPrefab;
    public GameObject scoutPrefab;
    public GameObject colonizerPrefab;

    private static int Year;

    HexGrid grid;

    // Use this for initialization
    void Start()
    {
        grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();

        InitPlayers();
        InitMap();
        InitSpaceships();
        Year = 0;
        currentPlayerIndex--;
        NextTurn();
    }

    void InitPlayers()
    {
        // Create players from prefab.
        // todo: should be done after main menu
        players = new GameObject[3];
        players[0] = Instantiate(playerPrefab);
        players[0].GetComponent<Player>().human = true;
        players[0].name = "Main Player";

        for (int i = 1; i < 3; i++)
        {
            players[i] = Instantiate(playerPrefab);
            players[i].GetComponent<Player>().human = false;
            players[i].name = "AI-" + i;
        }

        currentPlayerIndex = 0;
    }

    void InitSpaceships()
    {
        foreach(GameObject player in players)
        {
            Debug.Log("Init spaceship for player " + player.name);
            Planet homePlanet = player.GetComponent<Player>().GetPlanets().Cast<Planet>().First();
            Spaceship spaceship = SpaceshipFromPref(scoutPrefab, homePlanet);
            spaceship.Owned(player.GetComponent<Player>());
            Debug.Log("spaceship: " + spaceship.Coordinates);
        }
    }

    HexCell EmptyCell(HexCoordinates startCooridantes)
    {
        // serch for empty hexCell
        HexCell cell;
        for (int X = -1; X <= 1; X+=2)
        {
            for (int Z = -1; Z <= 1; Z+=2)
            {
                HexCoordinates newCoordinates = new HexCoordinates(startCooridantes.X + X, startCooridantes.Z + Z);
                cell = grid.FromCoordinates(newCoordinates);
                if (cell != null && cell.IsEmpty())
                    return cell;
            }
        }
        return null;
    }

    Spaceship SpaceshipFromPref(GameObject spaceshipPrefab, Planet startPlanet)
    {

        HexCoordinates homePlanetCoordinates = HexCoordinates.FromPosition(startPlanet.transform.position);
        HexCell spaceshipGrid = EmptyCell(homePlanetCoordinates);

        if (spaceshipGrid != null)
        {
            return Instantiate(spaceshipPrefab, spaceshipGrid.transform).GetComponent<Spaceship>();
        } else {
        Debug.Log("Can't find empty cell for ");
        }
        return null;
    }

    void InitMap()
    {
        // Create map from file / random.
        // todo: in main menu we should decide if map is from file or random and set parameters
        // todo: move json deserialization to Planet's FromJson method
        // serializacje w unity ssie, trzeba bedzie doprawcowac (potrzebne bedzie do save/load i pewnie networkingu...)
        // todo: w jsonach nie moze byc utf8

        JObject o = JObject.Parse(Resources.Load("map1").ToString());
        InitPlanets((JArray)o["planets"]);
        InitStars((JArray)o["stars"]);
    }

    void InitPlanets(JArray jPlanetsCollection)
    {
        int playersWithHomePLanet = 0;

        foreach (JObject jPlanetSerialized in jPlanetsCollection)
        {
            GameObject planet = Instantiate(original: planetPrefab, position: new Vector3(
                (float)jPlanetSerialized["position"][0], (float)jPlanetSerialized["position"][1], (float)jPlanetSerialized["position"][2]), rotation: Quaternion.identity
            );
            JsonUtility.FromJsonOverwrite(jPlanetSerialized["planetMain"].ToString(), planet.GetComponent<Planet>());
            planet.name = jPlanetSerialized["name"].ToString();

            float radius = (float)jPlanetSerialized["radius"];
            planet.GetComponent<SphereCollider>().radius = radius;
            planet.transform.localScale = new Vector3(radius, radius, radius);

            if ((bool)jPlanetSerialized["mayBeHome"] == true && playersWithHomePLanet < players.Length)
            {
                planet.GetComponent<Planet>().Colonize(players[playersWithHomePLanet].GetComponent<Player>());
                playersWithHomePLanet++;
            }
        } 

        if (playersWithHomePLanet < players.Length)
        {
            throw new Exception("Not enough planets for players");
        }
    }

    void InitStars(JArray jStarsCollection)
    {
        foreach (JObject jStarSerialized in jStarsCollection)
        {
            GameObject star = Instantiate(original: startPrefab, position: new Vector3(
                (float)jStarSerialized["position"][0], (float)jStarSerialized["position"][1], (float)jStarSerialized["position"][2]), rotation: Quaternion.identity
            );
            star.name = jStarSerialized["name"].ToString();

            float radius = (float)jStarSerialized["radius"];
            star.GetComponent<SphereCollider>().radius = radius;
            star.transform.localScale = new Vector3(radius, radius, radius);
        }
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        if(currentPlayerIndex == 0)
        {
            Year++;
            Debug.Log("New year: " + Year);
        }

        EventManager.selectionManager.SelectedObject = null;
        grid.SetupNewTurn(GetCurrentPlayer());
        GameObject.Find("MiniMap").GetComponent<MiniMapController>().SetupNewTurn(GetCurrentPlayer());

        Debug.Log("Next turn, player: " + GetCurrentPlayer().name);
    }

    public static Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex].GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
