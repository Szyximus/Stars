using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System;

public class GameController : MonoBehaviour
{
    private Player[] players;
    public Player playerPrefab;
    private int currentPlayerIndex;

    public Planet planetPrefab;
    public Star startPrefab;

    // Use this for initialization
    void Start()
    {
        InitPlayers();
        InitMap();
    }

    void InitPlayers()
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

    void InitMap()
    {
        // Create map from file / random.
        // todo: in main menu we should decide if map is from file or random and set parameters
        // todo: move json deserialization to Planet's fromJson method
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
            Debug.Log(jPlanetSerialized);
            Planet planet = Instantiate(original: planetPrefab, position: new Vector3(
                (float)jPlanetSerialized["position"][0], (float)jPlanetSerialized["position"][1], (float)jPlanetSerialized["position"][2]), rotation: Quaternion.identity
            );
            JsonUtility.FromJsonOverwrite(jPlanetSerialized["planetMain"].ToString(), planet);
            planet.name = jPlanetSerialized["name"].ToString();
            planet.GetComponent<SphereCollider>().radius = (float)jPlanetSerialized["radius"];
            
            if((bool)jPlanetSerialized["mayBeHome"] == true && playersWithHomePLanet < players.Length)
            {
                planet.Owner = players[playersWithHomePLanet];
                planet.Colonized = true;
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
            Planet planet = Instantiate(original: planetPrefab, position: new Vector3(
                (float)jStarSerialized["position"][0], (float)jStarSerialized["position"][1], (float)jStarSerialized["position"][2]), rotation: Quaternion.identity
            );
            planet.name = jStarSerialized["name"].ToString();
            planet.GetComponent<SphereCollider>().radius = (float)jStarSerialized["radius"];
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
