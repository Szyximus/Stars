using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using UnityEditor;

public class GameController : MonoBehaviour
{
    private static List<GameObject> players;
    public GameObject PlayerPrefab;
    private static int currentPlayerIndex;

    private List<GameObject> planets;
    private List<GameObject> stars;
    public List<GameObject> spaceships;

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
    private static readonly string configsPath = "Assets/Configs/Resources/";

    private GameApp gameApp;
    private LevelLoader levelLoader;

    public InputField SaveGameFileInput;


    void Awake()
    {
        grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        turnScreen = GameObject.Find("Canvas").GetComponentInChildren<TurnScreen>();
        turnScreen.gameObject.SetActive(false);

        players = new List<GameObject>();
        planets = new List<GameObject>();
        stars = new List<GameObject>();
        spaceships = new List<GameObject>();

        currentPlayerIndex = 0;

        Debug.Log("GameContoller awake");
    }

    void Start()
    {
        string savedGameFile = gameApp.GetAndRemoveInputField("SavedGameFile");
        if(savedGameFile == null || savedGameFile.Equals(""))
        {
            Debug.Log("savedGameFile empty");
            levelLoader.LoadLevel("MainMenuScene");
            return;
        }

        
        string path = configsPath + "/" + savedGameFile + ".json";
        StreamReader reader = new StreamReader(path);
        string savedGameContent = reader.ReadToEnd();
        reader.Close();

        if (savedGameContent == null || "".Equals(savedGameContent))
        {
            Debug.Log("savedGameContent is null, path: " + path);
            levelLoader.LoadLevel("MainMenuScene");
            return;
        }

        LoadMap(savedGameContent);
        StartGame();
    }

    public void Exit()
    {
        levelLoader.LoadLevel("MainMenuScene");
    }

    public void SaveGame()
    {
        string SaveGameFile = SaveGameFileInput.text;
        if (SaveGameFile == null || "".Equals(SaveGameFile))
            return;

        Debug.Log("Saving to file " + SaveGameFile);

        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();

            writer.WritePropertyName("players");
            writer.WriteRawValue(SavePlayers());

            writer.WritePropertyName("map");
            writer.WriteStartObject();

            writer.WritePropertyName("planets");
            writer.WriteRawValue(SavePlanets());

            writer.WritePropertyName("stars");
            writer.WriteRawValue(SaveStars());

            writer.WritePropertyName("spaceships");
            writer.WriteRawValue(SaveSpaceships());

            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        string path = configsPath + "/" + SaveGameFile + ".json";
        StreamWriter StreamWriter = new StreamWriter(path);
        StreamWriter.Write(sb.ToString());
        StreamWriter.Close();

        AssetDatabase.ImportAsset(path);
    }

    private string SavePlayers()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartArray();

            foreach (GameObject playerGameObject in players)
            {
                Player player = playerGameObject.GetComponent<Player>();
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                writer.WriteValue(playerGameObject.name);

                writer.WritePropertyName("playerMain");
                writer.WriteRawValue(JsonUtility.ToJson(player));

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
        return sb.ToString();
    }

    private string SavePlanets()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartArray();

            foreach (GameObject planetGameObject in planets)
            {

                Planet planet = planetGameObject.GetComponent<Planet>();
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                writer.WriteValue(planetGameObject.name);

                writer.WritePropertyName("owner");
                writer.WriteValue(planet.GetOwnerName());

                writer.WritePropertyName("planetMain");
                writer.WriteRawValue(JsonUtility.ToJson(planet));

                writer.WritePropertyName("radius");
                writer.WriteValue(planet.transform.localScale.x);

                writer.WritePropertyName("material");
                writer.WriteValue(planetGameObject.GetComponentsInChildren<MeshRenderer>()[0].material.name.Replace(" (Instance)", ""));

                writer.WritePropertyName("position");
                writer.WriteStartArray();
                writer.WriteRawValue(planetGameObject.transform.position.ToString().Substring(1, this.transform.position.ToString().Length - 2));
                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        return sb.ToString();
    }

    private string SaveStars()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartArray();

            foreach (GameObject starGameObject in stars)
            {
                Star star = starGameObject.GetComponent<Star>();
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                writer.WriteValue(starGameObject.name);

                writer.WritePropertyName("starMain");
                writer.WriteRawValue(JsonUtility.ToJson(star));

                writer.WritePropertyName("radius");
                writer.WriteValue(star.transform.localScale.x);

                writer.WritePropertyName("material");
                writer.WriteValue(starGameObject.GetComponentsInChildren<MeshRenderer>()[0].material.name.Replace(" (Instance)",""));

                writer.WritePropertyName("position");
                writer.WriteStartArray();
                writer.WriteRawValue(starGameObject.transform.position.ToString().Substring(1, this.transform.position.ToString().Length - 2));
                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
        return sb.ToString();
    }

    private string SaveSpaceships()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartArray();

            foreach (GameObject spaceshipGameObject in spaceships)
            {
                Spaceship spaceship = spaceshipGameObject.GetComponent<Spaceship>();
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                writer.WriteValue(spaceshipGameObject.name);

                writer.WritePropertyName("model");
                writer.WriteValue(spaceship.getModel());

                writer.WritePropertyName("owner");
                writer.WriteValue(spaceship.GetOwnerName());

                writer.WritePropertyName("spaceshipMain");
                writer.WriteRawValue(JsonUtility.ToJson(spaceship));

                writer.WritePropertyName("position");
                writer.WriteStartArray();
                writer.WriteRawValue(spaceshipGameObject.transform.position.ToString().Substring(1, this.transform.position.ToString().Length - 2));
                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
        return sb.ToString();
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

        LoadPlayers((JArray)savedGameJson["players"]);

        JObject mapJson = (JObject)savedGameJson["map"];
        LoadPlanets((JArray)mapJson["planets"]);
        LoadStars((JArray)mapJson["stars"]);
        LoadSpaceships((JArray)mapJson["spaceships"]);
    }


    void LoadPlayers(JArray playersJson)
    {
        foreach (JObject playerJson in playersJson)
        {
            // init
            GameObject player = Instantiate(PlayerPrefab);
            JsonUtility.FromJsonOverwrite(playerJson["playerMain"].ToString(), player.GetComponent<Player>());

            // general
            player.name = (string)playerJson["name"];

            players.Add(player);
        }
    }

    void LoadSpaceships(JArray spaceshipsJson)
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
            GameObject spaceship = Instantiate(original: FindPrefab((string)spaceshipJson["model"]), position: new Vector3(
                (float)spaceshipJson["position"][0], (float)spaceshipJson["position"][1], (float)spaceshipJson["position"][2]), rotation: Quaternion.identity
            );
            JsonUtility.FromJsonOverwrite(spaceshipJson["spaceshipMain"].ToString(), spaceship.GetComponent<Spaceship>());

            // general
            spaceship.name = spaceshipJson["model"] + "-" + counter;

            // references and owner
            spaceship.GetComponent<Spaceship>().Owned(player);

            spaceships.Add(spaceship);
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

    

    void LoadPlanets(JArray planetsJson)
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
            if (planetJson["owner"] != null && !"".Equals(planetJson["owner"]))
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
                Material newMaterial = Resources.Load(materialString.Replace(" (Instance)", ""), typeof(Material)) as Material;
                if (materialString != null)
                {
                    planet.GetComponentsInChildren<MeshRenderer>()[0].material = newMaterial;
                    planet.GetComponentsInChildren<MeshRenderer>()[0].material.name = materialString;
                }
            }

            planets.Add(planet);
        }
    }

    void LoadStars(JArray starsJson)
    {
        foreach (JObject starJson in starsJson)
        {
            // init
            GameObject star = Instantiate(original: StartPrefab, position: new Vector3(
                (float)starJson["position"][0], (float)starJson["position"][1], (float)starJson["position"][2]), rotation: Quaternion.identity
            );
            JsonUtility.FromJsonOverwrite(starJson["starMain"].ToString(), star.GetComponent<Star>());

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
                Material newMaterial = Resources.Load(materialString.Replace(" (Instance)", ""), typeof(Material)) as Material;
                if (materialString != null)
                {
                    star.GetComponentsInChildren<MeshRenderer>()[0].material = newMaterial;
                    star.GetComponentsInChildren<MeshRenderer>()[0].material.name = materialString;
                }
            }

            stars.Add(star);
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
        GameObject player = players.Find(p => p.name == name);
        if(player != null)
            return player.GetComponent<Player>();
        return null;
    }

    public void LockInput()
    {
        turnScreen.gameObject.SetActive(true);
    }

    public void UnlockInput()
    {
        turnScreen.gameObject.SetActive(false);
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

                spaceships.Add(spaceship);
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
}
