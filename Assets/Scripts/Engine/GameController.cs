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
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

/*
 *  Object at "GameScene", server and clients should own a copy, so they can play as in local game
 *  Except starting game and "NextTurn" functions, which are different on server and clients
 */
public class GameController : NetworkBehaviour
{
    private List<GameObject> players;

    [SyncVar]
    public int currentPlayerIndex;

    private List<GameObject> planets;
    private List<GameObject> stars;
    public List<GameObject> spaceships;

    // current year in the game
    [SyncVar]
    public int year;

    private HexGrid grid;
    TurnScreen turnScreen;

    public GameApp gameApp;
    public LevelLoader levelLoader;

    // one of this is initialized from corresponding networkManagers
    public ClientNetworkManager clientNetworkManager;
    public ServerNetworkManager serverNetworkManager;

    public InputField SaveGameFileInput;

    void Awake()
    {
        Debug.Log("GameContoller Awake");

        grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        turnScreen = GameObject.Find("Canvas").GetComponentInChildren<TurnScreen>();

        players = new List<GameObject>();
        planets = new List<GameObject>();
        stars = new List<GameObject>();
        spaceships = new List<GameObject>();

        // init it here, because they depends on GameController, which is started after MonoBehaviour scripts
        GameObject.Find("UpperPanel").GetComponent<UpperPanel>().Init();
        GameObject.Find("SidePanel").GetComponent<SideMenu>().Init();
        turnScreen.Init();

        currentPlayerIndex = 0;
    }

    private void Start()
    {
        Debug.Log("GameContoller Start");
    }

    // start game
    /*
     *  Server only
     *  Called from ServerNetworkManager
     *  Starts new game, player names from the "NewGameScene" inputs, maps loaded is "map1"
     */
    public void ServerStartNewGame(List<GameApp.PlayerMenu> PlayerMenuList)
    {
        Debug.Log("ServerStartNewGame");
        if (!isServer)
        {
            throw new Exception("ServerStartNewGame not a server, return");
        }

        string path = gameApp.startMapsPath + "map1";
        Debug.Log("ServerStartNewGame reading " + path);

        TextAsset mapAsset = Resources.Load(path) as TextAsset;
        if(mapAsset == null)
        {
            throw new Exception("mapAsset is null");
        }

        string newGameContent = mapAsset.text;

        if (newGameContent == null || "".Equals(newGameContent))
        {
            throw new Exception("savedGameContent is null, path: " + path);
        }

        // replace player names
        newGameContent = PreparseStartMap(newGameContent, PlayerMenuList);

        JObject newGameJson = JObject.Parse(newGameContent);
        if (newGameJson == null)
        {
            throw new Exception("Error loading json");
        }

        PlayersFromMenu(PlayerMenuList);
        MapFromJson((JObject)newGameJson["map"]);
        InfoFromJson((JObject)newGameJson["info"]);

        // because "NextTurn" will increment
        currentPlayerIndex -= 1;
        NextTurn();
    }

    /*
     *  Server only
     *  Called from ServerNetworkManager
     *  Load game from json (as string)
     */
    public void ServerLoadGame(string savedGameContent)
    {
        Debug.Log("ServerLoadGame");
        if(!isServer)
        {
            throw new Exception("ServerLoadGame not a server, return");
        }

        GameFromJson(savedGameContent);

        // because "NextTurn" will increment
        currentPlayerIndex -= 1;
        NextTurn();
    }
    
    /*
    *  Server only
    *  Called from ServerNetworkManager
    *  Load game from json (as string)
    */
    public void ServerNextTurnGame(string savedGameContent)
    {
        Debug.Log("ServerLoadGame");

        GameFromJson(savedGameContent);

        // just make new turn, do not decrement currentPlayerId
        NextTurn();
    }

    /*
     *  "Exit" button in the game, do clean up
     */
    public void Exit()
    {
        Debug.Log("Exit");

        if (isServer)
            Destroy(serverNetworkManager);
        else if (isClient)
            Destroy(clientNetworkManager);
        else
            Debug.Log("wtf we are?");

        levelLoader.LoadLevel("MainMenuScene");
    }


    /*
     * Server only
     *  Called from "Save" button
     */
    public void SaveGame()
    {
        Debug.Log("SaveGame");
        if (!isServer)
        {
            Debug.Log("Client can't save the game");
            return;
        }

        string SaveGameFile = SaveGameFileInput.text;
        if (SaveGameFile == null || "".Equals(SaveGameFile))
        {
            Debug.Log("SaveGameFile null or empty");
            return;
        }

        string path = gameApp.savedGamesPath + "/" + SaveGameFile + ".json";
        Debug.Log("Saving to file: " + path);

        StreamWriter StreamWriter = new StreamWriter(path);
        StreamWriter.Write(GameToJson());
        StreamWriter.Close();
    }

    /*
     *  Make json with: info(year, currentPlayer), players and map(planets, stars, spaceships)
     */
    public string GameToJson()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();

            writer.WritePropertyName("players");
            writer.WriteRawValue(PlayersToJson());

            writer.WritePropertyName("info");
            writer.WriteRawValue(InfoToJson());

            writer.WritePropertyName("map");
            writer.WriteRawValue(MapToJson());

            writer.WriteEndObject();
        }
        return sb.ToString();
    }
    
    private string InfoToJson()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();

            writer.WritePropertyName("currentPlayer");
            writer.WriteValue(GetCurrentPlayer().name);

            writer.WritePropertyName("year");
            writer.WriteValue(year);

            writer.WriteEndObject();
        }
        return sb.ToString();
    }

    private string MapToJson()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();

            writer.WritePropertyName("planets");
            writer.WriteRawValue(PlanetsToJson());

            writer.WritePropertyName("stars");
            writer.WriteRawValue(StarsToJson());

            writer.WritePropertyName("spaceships");
            writer.WriteRawValue(SpaceshipsToJson());

            writer.WriteEndObject();
        }
        return sb.ToString();
    }

    private string PlayersToJson()
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

    private string PlanetsToJson()
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

    private string StarsToJson()
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

    private string SpaceshipsToJson()
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


    // game deserialization
    /*
     *  When creating new game, json with the game contains strings like "{{player1}}", "{{player3}}" etc
     *  They should be replaced with correct player names, indicating who own what and current player (first player)
     */
    string PreparseStartMap(string newGameContent, List<GameApp.PlayerMenu> PlayerMenuList)
    {
        int playerCounter = 1;
        foreach (GameApp.PlayerMenu playerMenu in PlayerMenuList)
        {
            newGameContent = newGameContent.Replace("{{player" + playerCounter + "}}", playerMenu.name);
            playerCounter += 1;
        }
        return newGameContent;
    }

    /*
     *  Setup variables in GameController, create objects on the map etc.
     */
    void GameFromJson(string savedGameContent)
    {
        // todo: w jsonach nie moze byc utf8

        JObject savedGameJson = JObject.Parse(savedGameContent);
        if(savedGameJson == null)
        {
            Debug.Log("Error loading json");
            return;
        }

        PlayersFromJson((JArray)savedGameJson["players"]);
        InfoFromJson((JObject)savedGameJson["info"]);
        MapFromJson((JObject)savedGameJson["map"]);
    }

    void InfoFromJson(JObject infoJson)
    {
        Debug.Log("InfoFromJson");
        year = (int)infoJson["year"];
        string currentPlayerName = (string)infoJson["currentPlayer"];
        if(FindPlayer(currentPlayerName))
        {
            currentPlayerIndex = players.IndexOf(FindPlayer(currentPlayerName).gameObject);
        }
        else
        {
            currentPlayerIndex = 0;
        }
    }

    void MapFromJson(JObject mapJson)
    {
        PlanetsFromJson((JArray)mapJson["planets"]);
        StarsFromJson((JArray)mapJson["stars"]);
        SpaceshipsFromJson((JArray)mapJson["spaceships"]);
    }

    void PlayersFromMenu(List<GameApp.PlayerMenu> PlayerMenuList)
    {
        foreach (GameApp.PlayerMenu playerMenu in PlayerMenuList)
        {
            // init
            GameObject playerGameObject = Instantiate(gameApp.PlayerPrefab);
            Player player = playerGameObject.GetComponent<Player>();

            // general
            player.name = playerMenu.name;
            player.password = playerMenu.password;
            player.human = playerMenu.isHuman;
            player.local = playerMenu.local.Equals("Y");

            players.Add(playerGameObject);
            NetworkServer.Spawn(playerGameObject);
        }
    }

    void PlayersFromJson(JArray playersJson)
    {
        foreach (JObject playerJson in playersJson)
        {
            // init
            GameObject player = Instantiate(gameApp.PlayerPrefab);
            JsonUtility.FromJsonOverwrite(playerJson["playerMain"].ToString(), player.GetComponent<Player>());

            // general
            player.name = (string)playerJson["name"];

            players.Add(player);
            NetworkServer.Spawn(player);
        }
    }

    void SpaceshipsFromJson(JArray spaceshipsJson)
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
            NetworkServer.Spawn(spaceship);
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

    void PlanetsFromJson(JArray planetsJson)
    {
        foreach (JObject planetJson in planetsJson)
        {
            // init
            GameObject planet = Instantiate(original: gameApp.PlanetPrefab, position: new Vector3(
                (float)planetJson["position"][0], (float)planetJson["position"][1], (float)planetJson["position"][2]), rotation: Quaternion.identity
            );
            JsonUtility.FromJsonOverwrite(planetJson["planetMain"].ToString(), planet.GetComponent<Planet>());
            NetworkServer.Spawn(planet);

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
            NetworkServer.Spawn(planet);
        }
    }

    void StarsFromJson(JArray starsJson)
    {
        foreach (JObject starJson in starsJson)
        {
            // init
            GameObject star = Instantiate(original: gameApp.StartPrefab, position: new Vector3(
                (float)starJson["position"][0], (float)starJson["position"][1], (float)starJson["position"][2]), rotation: Quaternion.identity
            );
            JsonUtility.FromJsonOverwrite(starJson["starMain"].ToString(), star.GetComponent<Star>());
            NetworkServer.Spawn(star);

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
            NetworkServer.Spawn(star);
        }
    }


    // game logic
    /*
     *  Make turns, different for server and remote clients
     */
    public void NextTurn()
    {
        Debug.Log("NextTurn");

        if (isServer)
            NextTurnServer();
        else
            NextTurnClient();
    }

    public void NextTurnClient()
    {
        Debug.Log("NextTurnClient");
        if (!isClient)
        {
            Debug.Log("NextTurnClient not a client, return");
            return;
        }

        // we have played locally, now serialize game and send to the server
        StringMessage clientMapJson = new StringMessage(GameToJson());
        clientNetworkManager.networkClient.Send(gameApp.connMapJsonId, clientMapJson);
    }

    public void NextTurnServer()
    {
        Debug.Log("NextTurnServer");
        if (!isServer)
        {
            Debug.Log("NextTurnServer not a server, return");
            return;
        }

        // change player
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count();
        if (currentPlayerIndex == 0)
        {
            year++;
            Debug.Log("New year: " + year);
        }

        // update objects for new turn
        foreach (Ownable owned in GetCurrentPlayer().GetOwned())
        {
            owned.SetupNewTurn();
        }

        EventManager.selectionManager.SelectedObject = null;
        grid.SetupNewTurn(GetCurrentPlayer());
        GameObject.Find("MiniMap").GetComponent<MiniMapController>().SetupNewTurn(GetCurrentPlayer());


        Debug.Log("Next turn, player: " + GetCurrentPlayer().name + ", local: " + GetCurrentPlayer().local);

        // set all clients to wait state
        NetworkServer.SetAllClientsNotReady();

        if (GetCurrentPlayer().local)
        {
            // local player turn, just play
            Debug.Log("Next local turn on server");
            turnScreen.Hide();
            turnScreen.Play("year: " + year + " | player: " + GetCurrentPlayer().name);
        }
        else
        {
            // now remote player turn, wait on the server
            Debug.Log("Next remote turn");
            turnScreen.Show("Waiting for player " + GetCurrentPlayer().name);

            // if client for the player is connected, set him ready and invoke "OnClientReady" message
            if (serverNetworkManager.connections.ContainsKey(GetCurrentPlayer().name))
            {
                NetworkConnection connection = serverNetworkManager.connections[GetCurrentPlayer().name];
                NetworkServer.SendToClient(connection.connectionId, gameApp.connSetupTurnId, new IntegerMessage(1));
                NetworkServer.SendToClient(connection.connectionId, gameApp.connClientLoadGameId, new StringMessage(GameToJson()));
            }

            // if client is not connected, we should have some "skip turn" button on the server
        }
    }


    // client game logic
    /*
     *  Called from clientNetworkManager, when the client should wait
     */
    public void WaitForTurn()
    {
        Debug.Log("WaitForTurn");
        turnScreen.Show("Waiting for our turn...");
    }

    /*
     * Called from clientNetworkManager, when the client should play
     */
    public void StopWaitForTurn()
    {
        Debug.Log("StopWaitForTurn");
        turnScreen.Hide();
    }


    // getters for basic info
    public Player GetCurrentPlayer()
    {
        if (players.Count == 0 || currentPlayerIndex >= players.Count || currentPlayerIndex < 0)
            return null;
        return players[currentPlayerIndex].GetComponent<Player>();
    }

    public int GetYear()
    {
        return year;
    }


    // helpers
    GameObject FindPrefab(string prefabName)
    {
        switch (prefabName)
        {
            case "Scout": return gameApp.ScoutPrefab;
            case "Miner": return gameApp.MinerPrefab;
            case "Warship": return gameApp.WarshipPrefab;
            case "Colonizer": return gameApp.ColonizerPrefab;
            default: return gameApp.ScoutPrefab;
        }
    }

    public Player FindPlayer(string name)
    {
        GameObject player = players.Find(p => p.name == name);
        if (player != null)
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


    // local game stuff
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
                NetworkServer.Spawn(spaceship);
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
