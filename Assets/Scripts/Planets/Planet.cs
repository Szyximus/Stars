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
using UnityEngine.Networking;


public class Planet : Ownable
{
    Random random = new Random();

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

    [SyncVar]
    public PlanetCharacteristics characteristics;
    [SyncVar]
    public PlanetResources resources;
    [SyncVar]
    public bool mayBeHome;

    [SyncVar]
    private int maxHealthPoints;

    private UIHoverListener uiListener;
    private HexGrid grid;
    public HexCoordinates Coordinates { get; set; }

    private new void Awake()
    {
        base.Awake();
        RadarRange = 40f;

        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        uiListener = GameObject.Find("Canvas").GetComponent<UIHoverListener>();

        maxHealthPoints = characteristics.healthPoints;
        UpdateCoordinates();

        Debug.Log("Awake planet " + name + ", coordinates: " + Coordinates + " - " + transform.position +
                   "Minerals " + resources.minerals + "HealthPoints " + characteristics.healthPoints);
    }


    void UpdateCoordinates()
    {
        Coordinates = HexCoordinates.FromPosition(gameObject.transform.position);
        if (grid.FromCoordinates(Coordinates) != null) transform.position = grid.FromCoordinates(Coordinates).transform.localPosition; //Snap object to hex
        if (grid.FromCoordinates(Coordinates) != null) grid.FromCoordinates(Coordinates).AssignObject(this.gameObject);
        //Debug.Log(grid.FromCoordinates(Coordinates).transform.localPosition.ToString() + '\n' + Coordinates.ToString());
    }

    private void OnMouseUpAsButton()
    {
        if (!uiListener.IsUIOverride && isActiveAndEnabled && grid.FromCoordinates(Coordinates).State == EHexState.Visible) EventManager.selectionManager.SelectedObject = this.gameObject;
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && isActiveAndEnabled &&
            EventManager.selectionManager.SelectedObject != null &&
            EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship != null &&
            EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>().GetOwner() != this.GetOwner())
        {
            Debug.Log("cel");
            EventManager.selectionManager.TargetObject = this.gameObject;
            Thread.Sleep(150);
        }
        else if (Input.GetMouseButtonDown(1) && EventManager.selectionManager.TargetObject == this.gameObject)
        {
            Debug.Log("tu nie");
            EventManager.selectionManager.TargetObject = null;
        }
    }


    override
    public void SetupNewTurn()
    {
        Player player = GetOwner();
        FindStarsNear();
        player.population += player.terraforming - characteristics.habitability + 2;
        player.minerals += resources.minerals / 3;
    }

    /**
     * Simple method to colonize planet.Sets the planet's owner specified in the method argument. 
     */
    public bool Colonize()
    {
        this.Colonize(gameController.GetCurrentPlayer());
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

        foreach (HexCoordinates offset in HexCoordinates.NeighboursOffsets)
        {
            HexCoordinates newCoordinates = new HexCoordinates(startCooridantes.X + offset.X, startCooridantes.Z + offset.Z);
            cell = grid.FromCoordinates(newCoordinates);
            if (cell != null && cell.IsEmpty())
                return cell;
        }
        return null;
    }

    public bool IsPossibleBuildSpaceship(Spaceship spaceship)
    {
        if (owner == gameController.GetCurrentPlayer())
            if (spaceship.neededMinerals <= GetOwner().minerals &&
            spaceship.neededPopulation <= GetOwner().population &&
            spaceship.neededSolarPower <= GetOwner().solarPower)
            {
                Debug.Log(spaceship.neededMinerals + "  " + GetOwner().minerals);
                Debug.Log("You have the required amount of resources");

                return true;
            }
        Debug.Log("You have not the required amount of resources");
        return false;
    }

    private void FindStarsNear()
    {
        List<GameObject> list;
        List<Star> starsList;
        Star starOneHex;
        Star starTwoHex;
        Star starThreeHex;

        var gameObjectsInProximityOneHex =
                Physics.OverlapSphere(transform.position, 10)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();
        var gameObjectsInProximityTwoHex =
                Physics.OverlapSphere(transform.position, 20)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();
        var gameObjectsInProximityThreeHex =
                Physics.OverlapSphere(transform.position, 150)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();

        var starsInProximityOneHex = gameObjectsInProximityOneHex.Where(o => o.tag == "Star");
        var starsInProximityTwoHex = gameObjectsInProximityTwoHex.Where(o => o.tag == "Star");
        var starsInProximityThreeHex = gameObjectsInProximityThreeHex.Where(o => o.tag == "Star");
        //zmienic ten syf
        try
        {
            //  list = starsInProximityOneHex.ToList();
            //if (list.Count() > 1)
            //{
            //    starsList.Add(list.)
            //}
            //   else if (list.Count() == 0)
            starOneHex = (starsInProximityOneHex.FirstOrDefault().GetComponent<Star>() as Star);
        }
        catch
        {
            starOneHex = null;
            Debug.Log("Near (one hex) the planet cannot find stars");

        }
        try
        {
            starTwoHex = (starsInProximityTwoHex.FirstOrDefault().GetComponent<Star>() as Star);
        }
        catch
        {
            starTwoHex = null;
            Debug.Log("Near (two hex)  the planet cannot find stars");
        }

        try
        {
            starThreeHex = (starsInProximityThreeHex.FirstOrDefault().GetComponent<Star>() as Star);
        }
        catch
        {
            starThreeHex = null;
            Debug.Log("Near (three hex) the planet cannot find stars");
        }

        Player player = GetOwner();

        if (starOneHex != null)
        {
            starOneHex.GiveSolarPower(GetOwner(), 3);
        }
        else if (starTwoHex != null)
        {
            starTwoHex.GiveSolarPower(GetOwner(), 2);
        }
        else if (starThreeHex != null)
        {
            starThreeHex.GiveSolarPower(GetOwner(), 1);
        }

    }

    private int GetMinerals(int mineralsCount)
    {
        if (resources.minerals >= mineralsCount)
            resources.minerals -= mineralsCount;

        return mineralsCount;
    }

    public bool GiveMineralsTo(Player player, int mineralsCount)
    {
        player.minerals += (GetMinerals(mineralsCount));
        return true;
    }

    public void AddHealthPoints(int healthPoints)
    {
        if ((this.characteristics.healthPoints += healthPoints) <= 0)
        {
            this.characteristics.healthPoints = maxHealthPoints;
            if (this.GetOwner() != null) Lose();
        }
        else
        {
            this.characteristics.healthPoints += healthPoints;
        }
    }
}