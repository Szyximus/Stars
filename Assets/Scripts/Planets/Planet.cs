using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Assets.Scripts.HexLogic;
using System.Linq;

public class Planet : Ownable
{
    Random rnd = new Random();
    [System.Serializable]
    public struct PlanetCharacteristics
    {
        public int habitability;
        public int healthPoints;
        public int temperature;
        public int radiation;
        public int oxygen;
    }

    [System.Serializable]
    public struct PlanetResources
    {
        public int minerals;
    }

    public PlanetCharacteristics characteristics;
    public PlanetResources resources;


    private UIHoverListener uiListener;
    private HexGrid grid;
    public HexCoordinates Coordinates { get; set; }

    private void Awake()
    {
        RadarRange = 40f;
    }

    void Start()
    {
        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        uiListener = GameObject.Find("Canvas").GetComponent<UIHoverListener>();

        UpdateCoordinates();
        Debug.Log("Start planet " + name + ", coordinates: " + Coordinates + " - " + transform.position +
                   "Minerals " + resources.minerals + "HealthPoints " + characteristics.healthPoints);
    }

    string ToJson()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();

            writer.WritePropertyName("planetMain");
            writer.WriteRawValue(JsonUtility.ToJson(this));

            writer.WritePropertyName("radius");
            writer.WriteValue(this.GetComponent<SphereCollider>().radius);

            writer.WritePropertyName("material");
            writer.WriteValue(this.GetComponentsInChildren<MeshRenderer>()[0].material);

            writer.WritePropertyName("position");
            writer.WriteStartArray();
            writer.WriteRawValue(this.transform.position.ToString().Substring(1, this.transform.position.ToString().Length - 2));
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
        return sb.ToString();
    }

    void FromJson(string json)
    {
        //JsonTextReader reader = new JsonTextReader(new StringReader(json));
        //while (reader.Read())
        //{
        //    if (reader.Value != null)
        //    {
        //        switch reader.
        //    }
        //    else
        //    {
        //        Console.WriteLine("Token: {0}", reader.TokenType);
        //    }
        //}
    }

    void UpdateCoordinates()
    {
        Coordinates = HexCoordinates.FromPosition(gameObject.transform.position);
        if (grid.FromCoordinates(Coordinates) != null) transform.position = grid.FromCoordinates(Coordinates).transform.localPosition; //Snap object to hex
        if (grid.FromCoordinates(Coordinates) != null) grid.FromCoordinates(Coordinates).AssignObject(this.gameObject);
        //Debug.Log(grid.FromCoordinates(Coordinates).transform.localPosition.ToString() + '\n' + Coordinates.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseUpAsButton()
    {
        if (!uiListener.IsUIOverride && isActiveAndEnabled && grid.FromCoordinates(Coordinates).State == EHexState.Visible) EventManager.selectionManager.SelectedObject = this.gameObject;
    }

    override
    public void SetupNewTurn()
    {
        FindStarsNear();
        GetOwner().minerals += 10;
        GetOwner().population += 10;
    }

    /**
     * Simple method to colonize planet.Sets the planet's owner specified in the method argument. 
     */
    public bool Colonize()
    {
        this.Colonize(GameController.GetCurrentPlayer());
        return true;
        //   Destroy(gameObject);
    }

    public void Colonize(Player newOnwer)
    {
        this.Owned(newOnwer);
        //   Destroy(gameObject);
    }
    public GameObject BuildSpaceship(GameObject spaceshipPrefab)
    {
        HexCoordinates homePlanetCoordinates = HexCoordinates.FromPosition(gameObject.transform.position);
        HexCell spaceshipGrid = EmptyCell(homePlanetCoordinates);

        if (spaceshipGrid != null)
        {
            return Instantiate(spaceshipPrefab, spaceshipGrid.transform.position, Quaternion.identity);//.GetComponent<Spaceship>();
        }
        else
        {
            Debug.Log("Can't find empty cell for spaceship " + spaceshipPrefab.name + " for planet " + gameObject.name);
        }
        return null;
    }
    HexCell EmptyCell(HexCoordinates startCooridantes)
    {
        // serch for empty hexCell
        HexCell cell;
        for (int X = -1; X <= 1; X += 2)
        {
            HexCoordinates newCoordinates = new HexCoordinates(startCooridantes.X + X, startCooridantes.Z);
            cell = grid.FromCoordinates(newCoordinates);
            if (cell != null && cell.IsEmpty())
                return cell;
        }
        //TODO to refactor. has been did only for demo
        if (true)
        {
            HexCoordinates newCoordinates = new HexCoordinates(startCooridantes.X - 1, startCooridantes.Z + 1);
            cell = grid.FromCoordinates(newCoordinates);
            if (cell != null && cell.IsEmpty())
                return cell;
        }
        if (true)
        {
            HexCoordinates newCoordinates = new HexCoordinates(startCooridantes.X + 1, startCooridantes.Z - 1);
            cell = grid.FromCoordinates(newCoordinates);
            if (cell != null && cell.IsEmpty())
                return cell;
        }
        //

        for (int Z = -1; Z <= 1; Z += 2)
        {
            HexCoordinates newCoordinates = new HexCoordinates(startCooridantes.X, startCooridantes.Z + Z);
            cell = grid.FromCoordinates(newCoordinates);
            if (cell != null && cell.IsEmpty())
                return cell;
        }
        return null;
    }
    public bool IsPossibleBuildSpaceship()
    {

        if (owner == GameController.GetCurrentPlayer())
        {
            //    if (owner.)
            //    {
            //        Debug.Log("You have the required amount of minerals");

            //    }

            return true;
        }
        Debug.Log("nie mozesz");
        return false;
    }
    private void FindStarsNear()
    {
        Star star;
        var gameObjectsInProximity =
                Physics.OverlapSphere(transform.position, 50)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();

        var cells = gameObjectsInProximity.Where(o => o.tag == "Star");
        try
        {
            star = (cells.FirstOrDefault().GetComponent<Star>() as Star);
        }
        catch
        {
            star = null;
            Debug.Log("Near the planet cannot find stars");
        }


        if (star != null)
        {
            Player player = GetOwner();
            player.power += 10;
        }

    }
    private int GetMinerals(int mineralsCount)
    {
        if (resources.minerals >= mineralsCount)
            resources.minerals -= mineralsCount;

        return mineralsCount;
    }
    public bool GiveMinerals(Player player)
    {
        player.minerals += (GetMinerals(40));
        return true;
    }
}