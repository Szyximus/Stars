using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System;

public class GameController : MonoBehaviour
{
    private GameObject[] players;
    public GameObject playerPrefab;
    private int currentPlayerIndex;

    public GameObject planetPrefab;
    public GameObject startPrefab;

    private int Year;

    // Use this for initialization
    void Start()
    {
        InitPlayers();
        InitMap();
        Year = 0;
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
            planet.GetComponent<SphereCollider>().radius = (float)jPlanetSerialized["radius"];
            
            if((bool)jPlanetSerialized["mayBeHome"] == true && playersWithHomePLanet < players.Length)
            {
                planet.GetComponent<Planet>().Owner = players[playersWithHomePLanet];
                planet.GetComponent<Planet>().Colonized = true;
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
            star.GetComponent<SphereCollider>().radius = (float)jStarSerialized["radius"];
        }
    }

    void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        if(currentPlayerIndex == 0)
        {
            Year++;
        }
    }

    Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex].GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
