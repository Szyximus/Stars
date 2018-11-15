/*
    Copyright (c) 2018, Szymon Jakóbczyk, Paweł Płatek, Michał Mielus, Maciej Rajs, Minh Nhật Trịnh, Izabela Musztyfaga
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation 
          and/or other materials provided with the distribution.
        * Neither the name of the [organization] nor the names of its contributors may be used to endorse or promote products derived from this software 
          without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
    USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

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
//using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Threading;
using System.IO.Compression;
//using System.Linq;

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

    public Stack<String> colorStack;

    // end game objectives
    public int tooRichTresholdMinerals, tooRichTresholdPopulation, tooRichTresholdSolarPower;

    void Awake()
    {
        Debug.Log("GameContoller Awake");
        colorStack = new Stack<string>();

        colorStack.Push("maroon");
        colorStack.Push("lime");
        colorStack.Push("magenta");
        colorStack.Push("orange");
        colorStack.Push("blue");
        colorStack.Push("yellow");
        colorStack.Push("cyan");
        colorStack.Push("red");

        grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        turnScreen = GameObject.Find("Canvas").GetComponentInChildren<TurnScreen>();

        // init it here, because they depends on GameController, which is started after MonoBehaviour scripts
        GameObject.Find("UpperPanel").GetComponent<UpperPanel>().Init();
        GameObject.Find("SidePanel").GetComponent<SideMenu>().Init();

        players = new List<GameObject>();
        planets = new List<GameObject>();
        stars = new List<GameObject>();
        spaceships = new List<GameObject>();

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
     *  Starts new game or loads game from file
     */
    public void ServerStartNewGame(bool isNewGame)
    {
        Debug.Log("ServerStartNewGame");
        if (!isServer)
        {
            throw new Exception("ServerStartNewGame not a server, return");
        }

        string path = null;
        if (isNewGame)
            path = gameApp.startMapsPath + gameApp.GetInputField("MapToLoad");
        else
            path = gameApp.savedGamesPath + gameApp.GetInputField("GameToLoad");
        JObject newGameJson = gameApp.ReadJsonFile(path);

        List<GameApp.PlayerMenu> PlayerMenuList = gameApp.GetAllPlayersFromMenu();

        PlayersFromJsonAndMenu(newGameJson, PlayerMenuList, isNewGame);
        MapFromJson((JObject)newGameJson["map"], isNewGame);
        InfoFromJson((JObject)newGameJson["info"], isNewGame);

        // because "NextTurn" will increment
        currentPlayerIndex -= 1;
        NextTurn();
    }

    /*
    *  Server only
    *  Called from ServerNetworkManager
    *  Load game from json (as string) from remote client (after his next turn)
    */
    public void ServerNextTurnGame(string savedGameContent)
    {
        Debug.Log("ServerLoadGame");
        if (!isServer)
        {
            throw new Exception("ServerNextTurnGame not a server, return");
        }

        GameFromJson(savedGameContent);

        // just make new turn, do not decrement currentPlayerId
        NextTurn();
    }

    /*
     *  Client only
     *  Called from ClientNetworkmanager
     *  Setup game locally from json received from server
     */
    public void ClientNextTurnGame(string savedGameContent)
    {
        Debug.Log("ClientNextTurnGame");
        if (!isClient)
        {
            throw new Exception("ClientNextTurnGame not a client, return");
        }

        GameFromJson(savedGameContent);

        // just make new turn, do not decrement currentPlayerId
        SetupNextTurnClient();
    }

    /*
     *  "Exit" button in the game, do clean up
     */
    public void Exit()
    {
        Debug.Log("Exit");

        if (isServer)
        {
            serverNetworkManager.StopServer();
        }
        else if (isClient)
        {
            if (clientNetworkManager == null)
                clientNetworkManager = GameObject.Find("ClientNetworkManager").GetComponent<ClientNetworkManager>();
            clientNetworkManager.StopClient();
        }
        else
        {
            Debug.Log("wtf we are?");
            levelLoader.Back("MainMenuScene");
        }
    }


    /*
     *  Called from "Save" button in "GameScene"
     */
    public void SaveGame()
    {
        Debug.Log("SaveGame");

        string SaveGameFile = SaveGameFileInput.text;
        if (SaveGameFile == null || "".Equals(SaveGameFile))
        {
            Debug.Log("SaveGameFile null or empty");
            return;
        }

        string path = gameApp.savedGamesPath + "/" + SaveGameFile;
        if (!path.EndsWith(".json"))
            path += ".json";
        Debug.Log("Saving to file: " + path);

        StreamWriter streamWriter = new StreamWriter(path);
        streamWriter.Write(GameToJson());
        streamWriter.Close();
    }

    /*
     *  Make json with: info(year, currentPlayer, maxPlayers), players and map(planets, stars, spaceships)
     *  maxPlayers - in new games its max players, in loading saved game it is actual number of players
     *  "owned" property in planets/spaceships - in new games it is number (from zero to maxPlayers), in saved games its user name as string
     */
    public string GameToJson()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
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
            writer.Formatting = Formatting.None;
            writer.WriteStartObject();

            writer.WritePropertyName("currentPlayer");
            writer.WriteValue(GetCurrentPlayer().name);

            writer.WritePropertyName("year");
            writer.WriteValue(year);

            writer.WritePropertyName("maxPlayers");
            writer.WriteValue(players.Count);

            // tooRichTreshold
            writer.WritePropertyName("tooRichTreshold");
            writer.WriteStartObject();

            writer.WritePropertyName("tooRichTresholdMinerals");
            writer.WriteValue(tooRichTresholdMinerals);

            writer.WritePropertyName("tooRichTresholdPopulation");
            writer.WriteValue(tooRichTresholdPopulation);

            writer.WritePropertyName("tooRichTresholdSolarPower");
            writer.WriteValue(tooRichTresholdSolarPower);

            writer.WriteEndObject();
            // tooRichTreshold end

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
            writer.Formatting = Formatting.None;
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
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();

            foreach (GameObject playerGameObject in players)
            {
                Player player = playerGameObject.GetComponent<Player>();
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                writer.WriteValue(playerGameObject.name);

                writer.WritePropertyName("color");
                writer.WriteValue(playerGameObject.GetComponent<Player>().color.ToString());

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
            writer.Formatting = Formatting.None;
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
                writer.WriteValue(planet.GetComponentsInChildren<MeshFilter>()[0].transform.localScale.x);

                writer.WritePropertyName("material");
                writer.WriteValue(planetGameObject.GetComponentsInChildren<MeshRenderer>()[0].material.name.Replace(" (Instance)", ""));

                writer.WritePropertyName("position");
                writer.WriteStartArray();
                writer.WriteRawValue(planetGameObject.transform.position.ToString().Replace("(", "").Replace(")", ""));
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
            writer.Formatting = Formatting.None;
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
                writer.WriteValue(star.GetComponentsInChildren<MeshFilter>()[0].transform.localScale.x);

                writer.WritePropertyName("material");
                writer.WriteValue(starGameObject.GetComponentsInChildren<MeshRenderer>()[0].material.name.Replace(" (Instance)", ""));

                writer.WritePropertyName("position");
                writer.WriteStartArray();
                writer.WriteRawValue(starGameObject.transform.position.ToString().Replace("(", "").Replace(")", ""));
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
            writer.Formatting = Formatting.None;
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
                writer.WriteRawValue(spaceshipGameObject.transform.position.ToString().Replace("(", "").Replace(")", ""));
                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
        return sb.ToString();
    }


    // game deserialization

    /*
     *  Setup variables in GameController, create objects on the map etc.
     */
    void GameFromJson(string savedGameContent)
    {
        // todo: w jsonach nie moze byc utf8

        JObject savedGameJson = JObject.Parse(savedGameContent);
        if (savedGameJson == null)
        {
            Debug.Log("Error loading json");
            return;
        }

        PlayersFromJson((JArray)savedGameJson["players"]);
        InfoFromJson((JObject)savedGameJson["info"], false);
        MapFromJson((JObject)savedGameJson["map"], false);
    }

    void InfoFromJson(JObject infoJson, bool isNewGame)
    {
        Debug.Log("InfoFromJson");
        year = (int)infoJson["year"];

        currentPlayerIndex = 0;
        if (isNewGame)
        {
            currentPlayerIndex = (int)infoJson["currentPlayer"];
        }
        else
        {
            string currentPlayerName = (string)infoJson["currentPlayer"];
            if (FindPlayer(currentPlayerName))
            {
                currentPlayerIndex = players.IndexOf(FindPlayer(currentPlayerName).gameObject);
            }
        }

        JObject tooRichTreshold = (JObject)infoJson["tooRichTreshold"];
        if (tooRichTreshold == null)
        {
            tooRichTresholdMinerals = 1000;
            tooRichTresholdPopulation = 1000;
            tooRichTresholdSolarPower = 1000;
        }
        else
        {
            tooRichTresholdMinerals = (int)tooRichTreshold["tooRichTresholdMinerals"];
            tooRichTresholdPopulation = (int)tooRichTreshold["tooRichTresholdPopulation"];
            tooRichTresholdSolarPower = (int)tooRichTreshold["tooRichTresholdSolarPower"];
        }
    }

    void MapFromJson(JObject mapJson, bool isNewGame)
    {
        PlanetsFromJson((JArray)mapJson["planets"], isNewGame);
        StarsFromJson((JArray)mapJson["stars"]);
        SpaceshipsFromJson((JArray)mapJson["spaceships"], isNewGame);
    }

    void PlayersFromJsonAndMenu(JObject gameJson, List<GameApp.PlayerMenu> PlayerMenuList, bool isNewGame)
    {
        Debug.Log(this.colorStack.Peek());
        JArray playersJson = null;

        if (!isNewGame)
        {
            if ((int)gameJson["info"]["maxPlayers"] != PlayerMenuList.Count)
                throw new Exception("Wrong number of players, should be " + (int)gameJson["info"]["maxPlayers"]);
            playersJson = (JArray)gameJson["players"];
            if (playersJson.Count != PlayerMenuList.Count)
                throw new Exception("Wrong number of players2, should be " + (int)gameJson["info"]["maxPlayers"]);

            // check if players names are not empty
            if (!playersJson.Where(s => ((string)s["name"]).Equals("")).ToList().Count.Equals(0))
            {
                throw new Exception("Players names can't be empty");
            }

            // check if players names are unique
            List<string> PlayerMenuListNames = playersJson.Select(s => (string)s["name"]).ToList();
            if (PlayerMenuListNames.Count != (new HashSet<string>(PlayerMenuListNames)).Count)
            {
                throw new Exception("Players names must be unique!");
            }
        }
        else
        {
            if ((int)gameJson["info"]["maxPlayers"] < PlayerMenuList.Count)
                throw new Exception("Too much players, max is " + (int)gameJson["info"]["maxPlayers"]);

            // check if players names are not empty
            if (!PlayerMenuList.Where(s => s.name.Equals("")).ToList().Count.Equals(0))
            {
                throw new Exception("Players names can't be empty");
            }

            // check if players names are unique
            List<string> PlayerMenuListNames = PlayerMenuList.Select(s => s.name).ToList();
            if (PlayerMenuListNames.Count != (new HashSet<string>(PlayerMenuListNames)).Count)
            {
                throw new Exception("Players names must be unique!");
            }
        }



        int i = 0;
        foreach (GameApp.PlayerMenu playerMenu in PlayerMenuList)
        {
            // init
            GameObject playerGameObject = Instantiate(gameApp.PlayerPrefab);
            Player player = playerGameObject.GetComponent<Player>();
            if (!isNewGame)
            {
                JsonUtility.FromJsonOverwrite(playersJson[i]["playerMain"].ToString(), player.GetComponent<Player>());
                player.name = (string)playersJson[i]["name"];
                ColorUtility.TryParseHtmlString((string)playersJson[i]["color"],out player.color);
            }
            else
            {
                player.name = playerMenu.name;
                player.race = playerMenu.race;

                var colorstring = this.colorStack.Pop();
                ColorUtility.TryParseHtmlString(colorstring, out player.color);
            }

            player.password = playerMenu.password;
            player.human = !playerMenu.playerType.Equals("A");
            player.local = !playerMenu.playerType.Equals("R");

            players.Add(playerGameObject);
            // NetworkServer.Spawn(playerGameObject);
            i++;
        }
    }

    void PlayersFromJson(JArray playersJson)
    {
        // check if players names are not empty
        if (!playersJson.Where(s => ((string)s["name"]).Equals("")).ToList().Count.Equals(0))
        {
            throw new Exception("Players names can't be empty");
        }

        // check if players names are unique
        List<string> PlayerMenuListNames = playersJson.Select(s => (string)s["name"]).ToList();
        if (PlayerMenuListNames.Count != (new HashSet<string>(PlayerMenuListNames)).Count)
        {
            throw new Exception("Players names must be unique!");
        }

        foreach (JObject playerJson in playersJson)
        {
            // init
            GameObject player = Instantiate(gameApp.PlayerPrefab);
            JsonUtility.FromJsonOverwrite(playerJson["playerMain"].ToString(), player.GetComponent<Player>());

            // general
            player.name = (string)playerJson["name"];
            ColorUtility.TryParseHtmlString((string)playerJson["color"], out player.GetComponent<Player>().color);

            players.Add(player);
            // NetworkServer.Spawn(player);
        }
    }

    void SpaceshipsFromJson(JArray spaceshipsJson, bool isNewGame)
    {
        int counter = 0;
        foreach (JObject spaceshipJson in spaceshipsJson)
        {
            counter += 1;

            // spaceship must have owner, check it first
            if (spaceshipJson["owner"] == null)
                continue;

            Player player = null;
            if (isNewGame)
            {
                // in new game, owner will contain number (from zero)
                if ((int)spaceshipJson["owner"] >= 0 && (int)spaceshipJson["owner"] < players.Count)
                {
                    player = players[(int)spaceshipJson["owner"]].GetComponent<Player>();
                }
            }
            else
            {
                // in saved game files, owner will contain player's name
                player = FindPlayer((string)spaceshipJson["owner"]);
            }

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
            // NetworkServer.Spawn(spaceship);
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

    void PlanetsFromJson(JArray planetsJson, bool isNewGame)
    {
        foreach (JObject planetJson in planetsJson)
        {
            // init
            GameObject planet = Instantiate(original: gameApp.PlanetPrefab, position: new Vector3(
                (float)planetJson["position"][0], (float)planetJson["position"][1], (float)planetJson["position"][2]), rotation: Quaternion.identity
            );

            JsonUtility.FromJsonOverwrite(planetJson["planetMain"].ToString(), planet.GetComponent<Planet>());
            // NetworkServer.Spawn(planet);

            // general
            planet.name = planetJson["name"].ToString();

            // references and owner
            if (isNewGame)
            {
                // in new game, owner will contain number (from zero)
                if (planetJson["owner"] != null && (int)planetJson["owner"] >= 0 && (int)planetJson["owner"] < players.Count)
                {
                    Player player = players[(int)planetJson["owner"]].GetComponent<Player>();
                    if (player != null)
                        planet.GetComponent<Planet>().Owned(player);
                }
            }
            else
            {
                // in saved game files, owner will contain player's name
                if (planetJson["owner"] != null && !"".Equals(planetJson["owner"]))
                {
                    Player player = FindPlayer((string)planetJson["owner"]);
                    if (player != null)
                        planet.GetComponent<Planet>().Owned(player);
                }
            }


            // mesh properties
            float radius = (float)planetJson["radius"];
            planet.GetComponentsInChildren<MeshFilter>()[0].transform.localScale = new Vector3(radius, radius, radius);

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
            // NetworkServer.Spawn(planet);
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
            // NetworkServer.Spawn(star);

            // general
            star.name = starJson["name"].ToString();

            // mesh properties
            float radius = (float)starJson["radius"];
            star.GetComponentsInChildren<MeshFilter>()[0].transform.localScale = new Vector3(radius, radius, radius);

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
            // NetworkServer.Spawn(star);
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

    /*
     *  After "NextTurn" button on clients
     *  Serialize game and send to the server
     */
    public void NextTurnClient()
    {
        Debug.Log("NextTurnClient");
        if (!isClient)
        {
            Debug.Log("NextTurnClient not a client, return");
            return;
        }

        StringMessage mapJsonMessage = new StringMessage(Compress(GameToJson()));

        if (clientNetworkManager == null)
            clientNetworkManager = GameObject.Find("ClientNetworkManager").GetComponent<ClientNetworkManager>();
        clientNetworkManager.networkClient.Send(gameApp.connMapJsonId, mapJsonMessage);
    }

    /*
     *  Called after receiving and loading game from server's json
     */
    public void SetupNextTurnClient()
    {
        EventManager.selectionManager.SelectedObject = null;
        grid.SetupNewTurn(GetCurrentPlayer());
        GameObject.Find("MiniMap").GetComponent<MiniMapController>().SetupNewTurn(GetCurrentPlayer());
    }

    /*
     *  After "NextTurn" button on server or after receiving json from remote client
     */
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
        EventManager.selectionManager.TargetObject = null;
        grid.SetupNewTurn(GetCurrentPlayer());
        GameObject.Find("MiniMap").GetComponent<MiniMapController>().SetupNewTurn(GetCurrentPlayer());


        Debug.Log("Next turn, player: " + GetCurrentPlayer().name + ", local: " + GetCurrentPlayer().local);

        // set all clients to wait state
        foreach (GameObject playerGameObject in players)
        {
            Player player = playerGameObject.GetComponent<Player>();
            if (serverNetworkManager.connections.ContainsKey(player.name))
            {
                NetworkConnection connection = serverNetworkManager.connections[player.name];
                string turnStatusJson = JsonUtility.ToJson(new GameApp.TurnStatus
                {
                    status = 0,
                    msg = "Waiting for your turn..."
                });
                if (player.looser)
                {
                    turnStatusJson = JsonUtility.ToJson(new GameApp.TurnStatus
                    {
                        status = 2,
                        msg = "You lost!\nYear: " + GetYear()
                    });
                }
                NetworkServer.SendToClient(connection.connectionId, gameApp.connSetupTurnId, new StringMessage(turnStatusJson));
            }
        }

        // check if winner
        if (IsCurrentPlayerWinner())
        {
            EndGame();
            return;
        }

        // check if looser
        if (IsCurrentPlayerLooser())
        {
            NextTurnServer();
            return;
        }

        if (GetCurrentPlayer().local)
        {
            // local player turn, just play
            Debug.Log("Next local turn on server");
            turnScreen.Play("Year: " + year + "\n\nPlayer: " + GetCurrentPlayer().name);
        }
        else
        {
            // now remote player turn, wait on the server
            Debug.Log("Next remote turn");
            turnScreen.Show("Waiting for player " + GetCurrentPlayer().name + "...");

            // if client for the player is connected, set him ready and invoke "OnClientReady" message
            if (serverNetworkManager.connections.ContainsKey(GetCurrentPlayer().name))
            {
                NetworkConnection connection = serverNetworkManager.connections[GetCurrentPlayer().name];
                string turnStatusJson = JsonUtility.ToJson(new GameApp.TurnStatus
                {
                    status = 1,
                    msg = "Play"
                });

                StringMessage mapJsonMessage = new StringMessage(Compress(GameToJson()));
                NetworkServer.SendToClient(connection.connectionId, gameApp.connClientLoadGameId, mapJsonMessage);
                NetworkServer.SendToClient(connection.connectionId, gameApp.connSetupTurnId, new StringMessage(turnStatusJson));
            }

            // if client is not connected, we should have some "skip turn" button on the server
        }
    }

    /*
     *  Send connClientEndGame to all players and make end screen for server
     */
    private void EndGame()
    {
        turnScreen.Show("Year: " + year + "\nWinner: " + GetCurrentPlayer().name);
        foreach (GameObject playerGameObject in players)
        {
            Player player = playerGameObject.GetComponent<Player>();
            if (serverNetworkManager.connections.ContainsKey(player.name))
            {
                string msg = "You lost!\nYear: " + year + "\nWinner: " + GetCurrentPlayer().name;
                if (GetCurrentPlayer().name.Equals(player.name))
                    msg = "You win!\nYear: " + year;
                NetworkConnection connection = serverNetworkManager.connections[player.name];
                NetworkServer.SendToClient(connection.connectionId, gameApp.connClientEndGame, new StringMessage(msg));
            }
        }
    }

    private bool IsCurrentPlayerLooser()
    {
        Player player = GetCurrentPlayer();

        // was looser before
        if (player.looser == true)
            return true;

        // no planets and no colonizers
        foreach (Ownable owned in player.GetOwned())
        {
            if (owned.gameObject.GetComponent<Planet>() != null)
                return false;
            if (owned.gameObject.GetComponent<Colonizer>() != null)
                return false;
        }

        player.looser = true;
        return true;
    }

    private bool IsCurrentPlayerWinner()
    {
        Player player = GetCurrentPlayer();

        // last man standing, only if more than one player
        if (players.Count > 1)
        {
            var notLoosers = players.Where(p => p.GetComponent<Player>().looser == false).ToList();
            if (notLoosers.Count == 1 && notLoosers.ElementAt(0).GetComponent<Player>() == player)
            {
                return true;
            }
        }

        // too rich to loose
        if (player.minerals >= tooRichTresholdMinerals &&
            player.population >= tooRichTresholdPopulation &&
            player.solarPower >= tooRichTresholdSolarPower)
        {
            return true;
        }

        return false;
    }

    /*
     *  Called from clientNetworkManager, when the client should end game
     */
    public void GameEnded(string msg)
    {
        Debug.Log("GameEnded");
        if (turnScreen == null)
            turnScreen = GameObject.Find("Canvas").GetComponentInChildren<TurnScreen>();
        turnScreen.Show("END\n" + msg);
    }

    /*
     *  Called from clientNetworkManager, when the client should wait
     */
    public void WaitForTurn(string msg)
    {
        Debug.Log("WaitForTurn: " + msg);
        turnScreen.Show(msg);
    }

    /*
     * Called from clientNetworkManager, when the client should play
     */
    public void StopWaitForTurn(string msg)
    {
        Debug.Log("StopWaitForTurn: " + msg);
        turnScreen.Hide();
    }

    /*
     *  Called from clientNetworkManager, when the client lost game
     */
    public void LostTurn(string msg)
    {
        Debug.Log("LostTurn: " + msg);
        turnScreen.Show(msg);
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

    /*
     *  Used for displaying info on TurnScreen (between turns)
     */
    public string GetTurnStatusInfo()
    {
        string info = "Year: " + year + "\n\n";
        info += "OBJECTIVES:\n\n";
        info += "Collect at least:\n";
        info += "  - minerals: " + tooRichTresholdMinerals + "\n";
        info += "  - population: " + tooRichTresholdPopulation + "\n";
        info += "  - solar power: " + tooRichTresholdSolarPower + "\n";

        if (players.Count > 1)
        {
            info += "Or destroy all your enemies";
        }

        return info;
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

    public string Compress(string inputStr)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
        var outputStream = new MemoryStream();
        using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
            gZipStream.Write(inputBytes, 0, inputBytes.Length);

        var outputBytes = outputStream.ToArray();
        Debug.Log("Compressed from " + inputStr.Length + " to " + outputBytes.Length);
        var outputbase64 = Convert.ToBase64String(outputBytes);
        return outputbase64;
    }

    public string Decompress(string inputStr)
    {
        byte[] inputBytes = Convert.FromBase64String(inputStr);

        using (var inputStream = new MemoryStream(inputBytes))
        using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
        using (var streamReader = new StreamReader(gZipStream))
        {
            var decompressed = streamReader.ReadToEnd();
            Debug.Log("Decompressed from " + inputStr.Length + " to " + decompressed.Length);
            return decompressed;
        }
    }


    // local game stuff
    public void Colonize()
    {
        var colonizer = EventManager.selectionManager.SelectedObject.GetComponent<Colonizer>();
        Planet planetToColonize = EventManager.selectionManager.TargetObject.GetComponent<Planet>();
        if (colonizer != null && planetToColonize != null && colonizer.GetActionPoints() > 0)

            if (colonizer.Colonize(planetToColonize))

            {
                grid.FromCoordinates(colonizer.Coordinates).ClearObject();
                GetCurrentPlayer().Lose(colonizer);
                Destroy(colonizer.gameObject);

            }
        Thread.Sleep(100);
    }

    public void MakeAllience()
    {
        Planet planetToAllience = EventManager.selectionManager.SelectedObject.GetComponent<Planet>();
        if (planetToAllience != null)
        {
            Player playerToAllience = planetToAllience.GetOwner();
            GetCurrentPlayer().MakeAlliance(playerToAllience);
            playerToAllience.MakeAlliance(GetCurrentPlayer());
        }
    }

    public void Mine()
    {
        var miner = EventManager.selectionManager.SelectedObject.GetComponent<Miner>();
        if (EventManager.selectionManager.TargetObject != null &&
           EventManager.selectionManager.TargetObject.GetComponent<Planet>() != null && miner.GetActionPoints() > 0)
        {
            Planet planetToMine = EventManager.selectionManager.TargetObject.GetComponent<Planet>();
            miner.MinePlanet(planetToMine);
            miner.SetActionPoints(-1);
        }
        else if (EventManager.selectionManager.TargetObject != null &&
             EventManager.selectionManager.TargetObject.GetComponent<Star>() != null && miner.GetActionPoints() > 0)
        {
            Star starToMine = EventManager.selectionManager.TargetObject.GetComponent<Star>();
            miner.MineStar(starToMine);
            miner.SetActionPoints(-1);
        }
        Thread.Sleep(150);
    }

    public void Attack()
    {
        var spaceship = EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>();
        Ownable target = EventManager.selectionManager.TargetObject.GetComponent<Ownable>();

        if (spaceship != null && spaceship.GetActionPoints() > 0 && target != null)

        {
            if (spaceship.Attack(target))
            {
                Debug.Log("You attacked");
                spaceship.SetActionPoints(-1);
            }
        }
        Thread.Sleep(150);
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
                // NetworkServer.Spawn(spaceship);
            }
        }
    }

    public void AddTerraforming()
    {
        if (GetCurrentPlayer().terraforming < 3 &&
            GetCurrentPlayer().minerals >= GetCurrentPlayer().researchStruct.terraformingNeededMinerals &&
            GetCurrentPlayer().population >= GetCurrentPlayer().researchStruct.terraformingNeededPopulation &&
            GetCurrentPlayer().solarPower >= GetCurrentPlayer().researchStruct.terraformingNeededSolarPower)
        {
            GetCurrentPlayer().minerals -= GetCurrentPlayer().researchStruct.terraformingNeededMinerals;
            GetCurrentPlayer().population -= GetCurrentPlayer().researchStruct.terraformingNeededPopulation;
            GetCurrentPlayer().solarPower -= GetCurrentPlayer().researchStruct.terraformingNeededSolarPower;

            GetCurrentPlayer().researchStruct.terraformingNeededMinerals += 5;
            GetCurrentPlayer().researchStruct.terraformingNeededPopulation += 5;
            GetCurrentPlayer().researchStruct.terraformingNeededSolarPower += 5;
            GetCurrentPlayer().researchStruct.terraformingLevel += 1;
            GetCurrentPlayer().terraforming++;
        }
    }

    public void AddAttack()
    {
        if (GetCurrentPlayer().attack < 3 &&
            GetCurrentPlayer().minerals >= GetCurrentPlayer().researchStruct.attackNeededMinerals &&
            GetCurrentPlayer().population >= GetCurrentPlayer().researchStruct.attackNeededPopulation &&
            GetCurrentPlayer().solarPower >= GetCurrentPlayer().researchStruct.attackNeededSolarPower)
        {
            GetCurrentPlayer().minerals -= GetCurrentPlayer().researchStruct.attackNeededMinerals;
            GetCurrentPlayer().population -= GetCurrentPlayer().researchStruct.attackNeededPopulation;
            GetCurrentPlayer().solarPower -= GetCurrentPlayer().researchStruct.attackNeededSolarPower;

            GetCurrentPlayer().researchStruct.attackNeededMinerals += 5;
            GetCurrentPlayer().researchStruct.attackNeededPopulation += 5;
            GetCurrentPlayer().researchStruct.attackNeededSolarPower += 5;
            GetCurrentPlayer().researchStruct.attackLevel += 1;
            GetCurrentPlayer().attack++;
        }
    }

    public void AddEngines()
    {
        if (GetCurrentPlayer().engines < 3 &&
            GetCurrentPlayer().minerals >= GetCurrentPlayer().researchStruct.enginesNeededMinerals &&
            GetCurrentPlayer().population >= GetCurrentPlayer().researchStruct.enginesNeedesPopulation &&
            GetCurrentPlayer().solarPower >= GetCurrentPlayer().researchStruct.enginesNeededSolarPower)
        {
            GetCurrentPlayer().minerals -= GetCurrentPlayer().researchStruct.enginesNeededMinerals;
            GetCurrentPlayer().population -= GetCurrentPlayer().researchStruct.enginesNeedesPopulation;
            GetCurrentPlayer().solarPower -= GetCurrentPlayer().researchStruct.enginesNeededSolarPower;

            GetCurrentPlayer().researchStruct.enginesNeededMinerals += 5;
            GetCurrentPlayer().researchStruct.enginesNeedesPopulation += 5;
            GetCurrentPlayer().researchStruct.enginesNeededSolarPower += 5;
            GetCurrentPlayer().researchStruct.enginesLevel += 1;
            GetCurrentPlayer().engines++;

        }
    }

    public void AddRadars()
    {
        if (GetCurrentPlayer().radars < 3 &&
            GetCurrentPlayer().minerals >= GetCurrentPlayer().researchStruct.radarsNeededMinerals &&
            GetCurrentPlayer().population >= GetCurrentPlayer().researchStruct.radarsNeededPopulation &&
            GetCurrentPlayer().solarPower >= GetCurrentPlayer().researchStruct.radarsNeededSolarPower)
        {
            GetCurrentPlayer().minerals -= GetCurrentPlayer().researchStruct.radarsNeededMinerals;
            GetCurrentPlayer().population -= GetCurrentPlayer().researchStruct.radarsNeededPopulation;
            GetCurrentPlayer().solarPower -= GetCurrentPlayer().researchStruct.radarsNeededSolarPower;

            GetCurrentPlayer().researchStruct.radarsNeededMinerals += 5;
            GetCurrentPlayer().researchStruct.radarsNeededPopulation += 5;
            GetCurrentPlayer().researchStruct.radarsNeededSolarPower += 5;
            GetCurrentPlayer().researchStruct.radarsLevel += 1;
            GetCurrentPlayer().radars++;
        }
    }

}

