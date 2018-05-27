using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Assets.Scripts;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private static List<GameObject> players;
    public GameObject PlayerPrefab;
    private static int currentPlayerIndex;

    public GameObject PlanetPrefab;
    public GameObject StartPrefab;
    public GameObject ScoutPrefab;
    public GameObject ColonizerPrefab;
    public GameObject MinerPrefab;
    public GameObject WarshipPrefab;

    public GameObject ExplosionPrefab;
    public GameObject AttackPrefab;
    public GameObject HitPrefab;

    private static int year;

    private HexGrid grid;
    TurnScreen turnScreen;

    private GameApp gameApp;
    private LevelLoader levelLoader;

    void Awake()
    {
        grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        turnScreen = GameObject.Find("Canvas").GetComponentInChildren<TurnScreen>();
        turnScreen.gameObject.SetActive(false);

        players = new List<GameObject>();
        currentPlayerIndex = 0;

        Debug.Log("GameContoller awake");
    }

    void Start()
    {
        string savedGameFile = gameApp.Parameters["SavedGameFile"];
        if(savedGameFile == null || savedGameFile.Equals(""))
        {
            Debug.Log("savedGameFile empty");
            levelLoader.LoadLevel("MainMenuScene");
            return;
        }

        System.Object savedGameResource = Resources.Load(savedGameFile);
        if(savedGameResource == null)
        {
            Debug.Log("savedGameResource is null, savedGameFile: " + savedGameFile);
            levelLoader.LoadLevel("MainMenuScene");
            return;
        }

        LoadMap(savedGameResource.ToString());
        StartGame();
    }

    public void LockInput()
    {
        turnScreen.gameObject.SetActive(true);
    }

    public void UnlockInput()
    {
        turnScreen.gameObject.SetActive(false);
    }

    void LoadMap(string savedGame)
    {
        // todo: w jsonach nie moze byc utf8

        JObject savedGameJson = JObject.Parse(savedGame);
        if(savedGameJson == null)
        {
            Debug.Log("Error loading json");
            return;
        }

        InitPlayers((JArray)savedGameJson["players"]);

        JObject mapJson = (JObject)savedGameJson["map"];
        InitPlanets((JArray)mapJson["planets"]);
        InitStars((JArray)mapJson["stars"]);
        InitSpaceships((JArray)mapJson["spaceships"]);
    }


    void InitPlayers(JArray playersJson)
    {
        foreach (JObject playerJson in playersJson)
        {
            players.Add(Instantiate(PlayerPrefab));
            players.Last().GetComponent<Player>().human = (bool)playerJson["human"];
            players.Last().name = (string)playerJson["name"];
        }
    }

    void InitSpaceships(JArray spaceshipsJson)
    {
        int counter = 0;
        foreach (JObject spaceshipJson in spaceshipsJson)
        {
            counter += 1;

            // spaceship must have owner, check it first
            if (spaceshipJson["owner"] == null)
                continue;
            Player player = FindPlayer((string)spaceshipJson["owner"]);
            if (player == null)
                continue;

            // init
            GameObject spaceship = Instantiate(original: FindPrefab((string)spaceshipJson["type"]), position: new Vector3(
                (float)spaceshipJson["position"][0], (float)spaceshipJson["position"][1], (float)spaceshipJson["position"][2]), rotation: Quaternion.identity
            );
            JsonUtility.FromJsonOverwrite(spaceshipJson["spaceshipMain"].ToString(), spaceship.GetComponent<Spaceship>());

            // general
            spaceship.name = spaceshipJson["type"] + "-" + counter;

            // references and owner
            spaceship.GetComponent<Spaceship>().Owned(player);
        }
    }

    HexCell EmptyCell(HexCoordinates startCooridantes)
    {
        // serch for empty hexCell
        HexCell cell;
        
        foreach (HexCoordinates offset in HexCoordinates.NeighboursOffsets)
        {
                HexCoordinates newCoordinates = new HexCoordinates(startCooridantes.X + offset.X, startCooridantes.Z + offset.Z);
                cell = grid.FromCoordinates(newCoordinates);
                if (cell != null && cell.IsEmpty())
                    return cell;
        }
        return null;
    }

    GameObject SpaceshipFromPref(GameObject spaceshipPrefab, Planet startPlanet)
    {

        HexCoordinates homePlanetCoordinates = HexCoordinates.FromPosition(startPlanet.transform.position);
        HexCell spaceshipGrid = EmptyCell(homePlanetCoordinates);

        if (spaceshipGrid != null)
        {
            return Instantiate(spaceshipPrefab, spaceshipGrid.transform.position, Quaternion.identity);//.GetComponent<Spaceship>();
        }
        else
        {
            Debug.Log("Can't find empty cell for spaceship " + spaceshipPrefab.name + " for planet " + startPlanet.name);
        }
        return null;
    }

    

    void InitPlanets(JArray planetsJson)
    {
        foreach (JObject planetJson in planetsJson)
        {
            // init
            GameObject planet = Instantiate(original: PlanetPrefab, position: new Vector3(
                (float)planetJson["position"][0], (float)planetJson["position"][1], (float)planetJson["position"][2]), rotation: Quaternion.identity
            );
            JsonUtility.FromJsonOverwrite(planetJson["planetMain"].ToString(), planet.GetComponent<Planet>());

            // general
            planet.name = planetJson["name"].ToString();

            // references and owner
            if (planetJson["owner"] != null)
            {
                Player player = FindPlayer((string)planetJson["owner"]);
                if(player != null)
                    planet.GetComponent<Planet>().Colonize(player);
            }

            // mesh properties
            float radius = (float)planetJson["radius"];
            if(radius >= 1)
                planet.GetComponent<SphereCollider>().radius = radius;
            planet.transform.localScale = new Vector3(radius, radius, radius);

            string materialString = (string)planetJson["material"];
            if (materialString != null)
            {
                Material newMaterial = Resources.Load(materialString, typeof(Material)) as Material;
                if (materialString != null)
                    planet.GetComponentsInChildren<MeshRenderer>()[0].material = newMaterial;
            }
        }
    }

    void InitStars(JArray starsJson)
    {
        foreach (JObject starJson in starsJson)
        {
            // init
            GameObject star = Instantiate(original: StartPrefab, position: new Vector3(
                (float)starJson["position"][0], (float)starJson["position"][1], (float)starJson["position"][2]), rotation: Quaternion.identity
            );

            // general
            star.name = starJson["name"].ToString();

            // mesh properties
            float radius = (float)starJson["radius"];
            if (radius >= 1)
                star.GetComponent<SphereCollider>().radius = radius;
            star.transform.localScale = new Vector3(radius, radius, radius);

            string materialString = (string)starJson["material"];
            if (materialString != null)
            {
                Material newMaterial = Resources.Load(materialString, typeof(Material)) as Material;
                if (materialString != null)
                    star.GetComponentsInChildren<MeshRenderer>()[0].material = newMaterial;
            }
        }
    }

    GameObject FindPrefab(string prefabName)
    {
        switch(prefabName)
        {
            case "Scout": return ScoutPrefab;
            case "Miner": return MinerPrefab;
            case "Warship": return WarshipPrefab;
            case "Colonizer": return ColonizerPrefab;
            default: return ScoutPrefab;
        }
    }

    Player FindPlayer(string name)
    {
        return players.Find(p => p.name == name).GetComponent<Player>();
    }

    void StartGame()
    {
        Debug.Log("Starting game");
        currentPlayerIndex = players.Count() - 1; // NextTurn will wrap index to zero at the beginning
        year = -1;  // NextTurn will increment Year at the beginning
        NextTurn();
    }

    public void NextTurn()
    {
        turnScreen.gameObject.SetActive(true);
        turnScreen.Play();
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count();
        if (currentPlayerIndex == 0)
        {
            year++;
            Debug.Log("New year: " + year);
        }

        foreach (Ownable owned in GetCurrentPlayer().GetOwned())
        {
            owned.SetupNewTurn();
        }

        EventManager.selectionManager.SelectedObject = null;
        grid.SetupNewTurn(GetCurrentPlayer());
        GameObject.Find("MiniMap").GetComponent<MiniMapController>().SetupNewTurn(GetCurrentPlayer());

        Debug.Log("Next turn, player: " + GetCurrentPlayer().name);

        foreach (Spaceship s in GetCurrentPlayer().GetSpaceships()) {
            Debug.Log("s " + s.name + ": "  + JsonUtility.ToJson(s));
        }

    }

    public static Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex].GetComponent<Player>();
    }

    public static int GetYear()
    {
        return year;
    }

    public void Colonize()
    {
        var colonizer = EventManager.selectionManager.SelectedObject.GetComponent<Colonizer>();
        if (colonizer != null)
        {
            if (colonizer.ColonizePlanet())
            {
                grid.FromCoordinates(colonizer.Coordinates).ClearObject();
                GetCurrentPlayer().Lose(colonizer);
                Destroy(colonizer.gameObject);

            }
        }

    }

    public void Mine()
    {
        var miner = EventManager.selectionManager.SelectedObject.GetComponent<Miner>();
        if (miner != null && miner.GetActionPoints() > 0)
        {
            if (miner.MineResources())
                miner.SetActionPoints(-1);
            else
            {
                Debug.Log("Cannot mine");
            }
        }
    }

    public void Attack()
    {
        var spaceship = EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>();

        if (spaceship != null && spaceship.GetActionPoints() > 0)
        {
            if (spaceship.Attack())
            {
                Debug.Log("You attacked");
                spaceship.SetActionPoints(-1);
            }
        }
    }
    public void BuildSpaceship(GameObject spaceshipPrefab)
    {
        var planet = EventManager.selectionManager.SelectedObject.GetComponent<Planet>();
        if (planet != null)
        {
            //wywalic tego mocka 
            GameObject mockSpaceship = new GameObject();

            mockSpaceship = Instantiate(spaceshipPrefab);
            mockSpaceship.gameObject.SetActive(false);
            mockSpaceship.gameObject.GetComponent<Spaceship>().enabled = false;

            if (planet.IsPossibleBuildSpaceship(mockSpaceship.GetComponent<Spaceship>() as Spaceship))
            {
                Debug.Log("Building " + spaceshipPrefab.name);
                GameObject spaceship = planet.BuildSpaceship(spaceshipPrefab);
                spaceship.GetComponent<Spaceship>().Owned(GetCurrentPlayer());

                GetCurrentPlayer().minerals -= (spaceship.GetComponent<Spaceship>() as Spaceship).neededMinerals;
                GetCurrentPlayer().population -= (spaceship.GetComponent<Spaceship>() as Spaceship).neededPopulation;
                GetCurrentPlayer().solarPower -= (spaceship.GetComponent<Spaceship>() as Spaceship).neededSolarPower;

                EventManager.selectionManager.SelectedObject = null;
                grid.SetupNewTurn(GetCurrentPlayer());
                GameObject.Find("MiniMap").GetComponent<MiniMapController>().SetupNewTurn(GetCurrentPlayer());

                Debug.Log("Built " + spaceshipPrefab.name);
            }
        }
    }

    public void AddTerraforming()
    {
        if (GetCurrentPlayer().terraforming < 3) GetCurrentPlayer().terraforming++;
    }
    public void AddAttack()
    {
        if (GetCurrentPlayer().attack < 2) GetCurrentPlayer().attack++;
    }
    public void AddEngines()
    {
        if (GetCurrentPlayer().engines < 2) GetCurrentPlayer().engines++;
    }
    public void AddRadars()
    {
        if (GetCurrentPlayer().radars < 2) GetCurrentPlayer().radars++;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
