using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
using System.Linq;
public class SideMenu : MonoBehaviour
{
    bool shown, animating = false;

    GameController gameController;
    private bool initialized = false;

    RectTransform rectTransform;
    GameObject NamePanel;
    GameObject PlanetCharacteristicsPanel;
    GameObject ShipCharacteristicsPanel;
    GameObject PlanetResourcesPanel;
    GameObject BuildPanel;
    GameObject ShipPanel;

    GameObject FreePlanetFill;
    GameObject NoSelectionFill;
    GameObject StarFill;

    Text label;
    Text planetResources;
    Text planetCharacteristics;
    Text yeilds;
    Text shipCharacteristics;
    Text energy;
    //Text shipHP;
    Text ownerName;

    Text scoutCosts;
    Text minerCosts;
    Text warshipCosts;
    Text colonizerCosts;

    Button colonizeButton;
    Button mineButton;
    Button attackButton;

    Image icon;

    // Use this for initialization
    public void Init()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        transform.position += new Vector3(384, 0, 0);

        NamePanel = GameObject.Find("NamePanel");
        PlanetCharacteristicsPanel = GameObject.Find("PlanetCharacteristicsPanel");
        ShipCharacteristicsPanel = GameObject.Find("ShipCharacteristicsPanel");
        PlanetResourcesPanel = GameObject.Find("PlanetResourcesPanel");
        BuildPanel = GameObject.Find("BuildPanel");
        ShipPanel = GameObject.Find("ShipPanel");

        FreePlanetFill = GameObject.Find("FreePlanetFill");
        NoSelectionFill = GameObject.Find("NoSelectionFill");
        StarFill = GameObject.Find("StarFill");

        label = NamePanel.GetComponentInChildren<Text>();
        planetResources = PlanetResourcesPanel.GetComponentsInChildren<Text>().Last();

        planetCharacteristics = PlanetCharacteristicsPanel.GetComponentsInChildren<Text>().First();
        yeilds = PlanetCharacteristicsPanel.GetComponentsInChildren<Text>().Last();
        shipCharacteristics = ShipCharacteristicsPanel.GetComponentsInChildren<Text>().Last();
        energy = ShipPanel.GetComponentInChildren<Text>();
        //shipHP = ShipPanel.GetComponentsInChildren<Text>()[1];
        colonizeButton = ShipPanel.GetComponentsInChildren<Button>()[1];
        mineButton = ShipPanel.GetComponentsInChildren<Button>().First();
        attackButton = ShipPanel.GetComponentsInChildren<Button>()[2];

        ownerName = NamePanel.GetComponentsInChildren<Text>().Last();

        scoutCosts = BuildPanel.GetComponentsInChildren<Text>()[1];
        minerCosts = BuildPanel.GetComponentsInChildren<Text>()[2];
        warshipCosts = BuildPanel.GetComponentsInChildren<Text>()[3];
        colonizerCosts = BuildPanel.GetComponentsInChildren<Text>()[4];

        icon = NamePanel.GetComponentsInChildren<Image>().Last();

        initialized = true;
    }
    void ShowNamePanel()
    {
        NamePanel.SetActive(true);
        NoSelectionFill.SetActive(false);

        label.text = EventManager.selectionManager.SelectedObject.name.Replace("(Clone)", "");

        icon.sprite = EventManager.selectionManager.SelectedObject.GetComponentInChildren<SpriteRenderer>().sprite;

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Ownable>() as Ownable) != null) //If ownable
        {
            ShowOwnerName();
        }
        else
        {
            HideOwnerName();
        }
    }

    void ShowOwnerName()
    {
        string _owner = (EventManager.selectionManager.SelectedObject.GetComponent<Ownable>() as Ownable).GetOwnerName();

        if (_owner == "")
            ownerName.text = "No Owner";
        else
            ownerName.text = _owner;
        ownerName.gameObject.SetActive(true);
    }

    void HideOwnerName()
    {
        ownerName.gameObject.SetActive(false);
    }
    void ShowStarPanels()
    {
        ShipPanel.SetActive(false);

        PlanetCharacteristicsPanel.SetActive(false);
        ShipCharacteristicsPanel.SetActive(false);
        PlanetResourcesPanel.SetActive(false);
        BuildPanel.SetActive(false);
        FreePlanetFill.SetActive(false);
        NoSelectionFill.SetActive(false);
        StarFill.SetActive(true);

    }

    void ShowShipPanel()
    {
        ShipPanel.SetActive(true);

        PlanetCharacteristicsPanel.SetActive(false);
        ShipCharacteristicsPanel.SetActive(true);
        PlanetResourcesPanel.SetActive(false);
        BuildPanel.SetActive(false);
        FreePlanetFill.SetActive(false);
        NoSelectionFill.SetActive(false);
        StarFill.SetActive(false);

        energy.text = "Energy: " + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).GetActionPoints().ToString();

        shipCharacteristics.text = ("FirePower: " + ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).spaceshipStatistics.attack + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).GetOwner().attack).ToString() + "\n" +
                                     //"Defense: " + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).spaceshipStatistics.defense.ToString() + "\n" +
                                     "Speed: " + ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).spaceshipStatistics.speed + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).GetOwner().attack).ToString() + "\n" +
                                     "HP: " + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).spaceshipStatistics.healthPoints.ToString()).Replace("\n", System.Environment.NewLine);

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Colonizer>() as Colonizer) != null &&
            EventManager.selectionManager.TargetObject != null && EventManager.selectionManager.TargetObject.GetComponent<Planet>() != null)
        {
            if (EventManager.selectionManager.SelectedObject.GetComponent<Colonizer>().CheckDistance(EventManager.selectionManager.TargetObject.GetComponent<Planet>()))
                colonizeButton.gameObject.SetActive(true);
            else
                colonizeButton.gameObject.SetActive(false);
        }
        else
        {
            colonizeButton.gameObject.SetActive(false);
        }

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Miner>() as Miner) != null &&
            EventManager.selectionManager.TargetObject != null && (EventManager.selectionManager.TargetObject.GetComponent<Planet>() != null ||
           (EventManager.selectionManager.TargetObject.GetComponent<Star>() != null)))
        {
            if (EventManager.selectionManager.SelectedObject.GetComponent<Miner>().CheckDistance(EventManager.selectionManager.TargetObject.GetComponent<Planet>()) ||
              (EventManager.selectionManager.SelectedObject.GetComponent<Miner>().CheckDistance(EventManager.selectionManager.TargetObject.GetComponent<Star>())))
                mineButton.gameObject.SetActive(true);
            else
                mineButton.gameObject.SetActive(false);
        }
        else
        {
            mineButton.gameObject.SetActive(false);
        }

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Warship>() as Warship) != null &&
            EventManager.selectionManager.TargetObject != null && ((EventManager.selectionManager.TargetObject.GetComponent<Spaceship>() != null) ||
            (EventManager.selectionManager.TargetObject.GetComponent<Planet>() != null)))
        {
            if (EventManager.selectionManager.SelectedObject.GetComponent<Warship>().CheckDistance(EventManager.selectionManager.TargetObject.GetComponent<Ownable>()))
                attackButton.gameObject.SetActive(true);
            else
                attackButton.gameObject.SetActive(false);
        }
        else
        {
            attackButton.gameObject.SetActive(false);
        }
    }



    void ShowEnemySpaceshipPanels()
    {
        ShipPanel.SetActive(false);

        PlanetCharacteristicsPanel.SetActive(false);
        PlanetResourcesPanel.SetActive(false);
        BuildPanel.SetActive(false);
        NoSelectionFill.SetActive(false);
        StarFill.SetActive(false);

        ShipCharacteristicsPanel.SetActive(true);
        shipCharacteristics.text = ("FirePower: " + ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).spaceshipStatistics.attack + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).GetOwner().attack).ToString() + "\n" +
                                 //"Defense: " + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).spaceshipStatistics.defense.ToString() + "\n" +
                                 "Speed: " + ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).spaceshipStatistics.speed + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).GetOwner().attack).ToString() + "\n" +
                                 "HP: " + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).spaceshipStatistics.healthPoints.ToString()).Replace("\n", System.Environment.NewLine);
        FreePlanetFill.SetActive(true);
    }

    void ShowOwnedPlanetPanels()
    {
        PlanetCharacteristicsPanel.SetActive(true);
        ShipCharacteristicsPanel.SetActive(false);
        PlanetResourcesPanel.SetActive(false);
        BuildPanel.SetActive(true);
        ShipPanel.SetActive(false);

        FreePlanetFill.SetActive(false);
        NoSelectionFill.SetActive(false);
        StarFill.SetActive(false);

        Planet planet = EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet;

        planetCharacteristics.text = ("Temperature: " + planet.characteristics.temperature.ToString() + "\n" +
                                     "Oxygen: " + planet.characteristics.oxygen.ToString() + "\n" +
                                     "Radiation: " + planet.characteristics.radiation.ToString() + "\n" +
                                     "Habitability: " + planet.characteristics.habitability.ToString() + "\n" +
                                     "HP: " + planet.characteristics.healthPoints.ToString()).Replace("\n", System.Environment.NewLine);

        yeilds.text = ("+" + planet.GetPopulationGrowth().ToString() + "\n" + "\n" +
                       "+" + planet.GetMineralsnGrowth().ToString() + "\n" + "\n" +
                       "+" + planet.GetSolarPowerGrowth().ToString() );

        scoutCosts.text = '-' + gameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededSolarPower.ToString() +
            "     " +
            '-' + gameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededMinerals.ToString() +
            "     " +
            '-' + gameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededPopulation.ToString();

        minerCosts.text = '-' + gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededSolarPower.ToString() +
            "     " +
            '-' + gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededMinerals.ToString() +
            "     " +
            '-' + gameController.GetCurrentPlayer().spaceshipsCosts.minerNeededPopulation.ToString();

        warshipCosts.text = '-' + gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededSolarPower.ToString() +
            "     " +
            '-' + gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededMinerals.ToString() +
            "     " +
            '-' + gameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededPopulation.ToString();


        colonizerCosts.text = '-' + gameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededSolarPower.ToString() +
            "     " +
            '-' + gameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededMinerals.ToString() +
            "     " +
            '-' + gameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededPopulation.ToString();



    }

    void ShowFreePlanetPanels()
    {

        PlanetCharacteristicsPanel.SetActive(true);
        PlanetResourcesPanel.SetActive(true);
        ShipCharacteristicsPanel.SetActive(false);
        BuildPanel.SetActive(false);
        ShipPanel.SetActive(false);

        FreePlanetFill.SetActive(true);
        NoSelectionFill.SetActive(false);
        StarFill.SetActive(false);

        Planet planet = EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet;
        planetCharacteristics.text = ("Temperature: " + planet.characteristics.temperature.ToString() + "\n" +
                                     "Oxygen: " + planet.characteristics.oxygen.ToString() + "\n" +
                                     "Radiation: " + planet.characteristics.radiation.ToString() + "\n" +
                                     "Habitability: " + planet.characteristics.habitability.ToString() + "\n" +
                                     "HP: " + planet.characteristics.healthPoints.ToString()).Replace("\n", System.Environment.NewLine);
        planetResources.text = "Minerals: " + planet.resources.minerals.ToString();

        yeilds.text = ("+" + planet.GetPopulationGrowth().ToString() + "\n" + "\n" +
                       "+" + planet.GetMineralsnGrowth().ToString() + "\n" + "\n" +
                       "+" + planet.GetSolarPowerGrowth().ToString());


    }

    // Update is called once per frame
    void Update()
    {
        if (initialized == false)
            return;

        if (EventManager.selectionManager.SelectedObject == null && shown && !animating) // No selection
        {
            NamePanel.SetActive(false);
            PlanetCharacteristicsPanel.SetActive(false);
            PlanetResourcesPanel.SetActive(false);
            BuildPanel.SetActive(false);
            ShipPanel.SetActive(false);

            NoSelectionFill.SetActive(true);
            FreePlanetFill.SetActive(false);
            StarFill.SetActive(false);

            StartCoroutine(Hide());
        }

        if (EventManager.selectionManager.SelectedObject != null && !shown && !animating) //selection, show menu
        {
            StartCoroutine(Show());
        }

        if (EventManager.selectionManager.SelectedObject != null &&
            ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship) != null) &&
            ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Ownable).GetOwner() == gameController.GetCurrentPlayer()))// if owed Spaceship
        {
            ShowNamePanel();
            ShowShipPanel();
        }

        if (EventManager.selectionManager.SelectedObject != null &&
            ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship) != null) &&
            ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Ownable).GetOwner() != gameController.GetCurrentPlayer()))// if enemy Spaceship
        {
            ShowNamePanel();
            ShowEnemySpaceshipPanels();
        }

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Star>() as Star) != null) //if Star
        {
            ShowNamePanel();
            ShowStarPanels();

        }


        if (EventManager.selectionManager.SelectedObject != null &&
            (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet) != null &&
            (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Ownable).GetOwner() == gameController.GetCurrentPlayer()) // if owned planet
        {
            ShowNamePanel();
            ShowOwnedPlanetPanels();
        }


        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet) != null &&
           (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Ownable).GetOwner() != gameController.GetCurrentPlayer()) //Free Planet
        {
            ShowNamePanel();
            ShowFreePlanetPanels();
        }
    }

    IEnumerator Show()
    {


        animating = true;
        float startTime = Time.time;
        Vector3 direction = new Vector3(-384, 0, 0);
        var endPos = transform.position + direction;

        while (Time.time - startTime < 0.25) //the movement takes exactly 0,25 s. regardless of framerate
        {

            transform.position += direction * Time.deltaTime * 4;
            yield return null;
        }
        transform.position = endPos;
        shown = true;
        animating = false;
    }

    IEnumerator Hide()
    {

        animating = true;
        float startTime = Time.time;
        Vector3 direction = new Vector3(384, 0, 0);
        var endPos = transform.position + direction;

        while (Time.time - startTime < 0.25) //the movement takes exactly 0,25 s. regardless of framerate
        {

            transform.position += direction * Time.deltaTime * 4;
            yield return null;
        }
        transform.position = endPos;
        shown = false;
        animating = false;
    }
}