using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
using System.Linq;
public class SideMenu : MonoBehaviour
{
    bool shown, animating = false;

    GameController GameController;

    RectTransform rectTransform;
    GameObject NamePanel;
    GameObject PlanetCharacteristicsPanel;
    GameObject PlanetResourcesPanel;
    GameObject BuildPanel;
    GameObject ShipPanel;

    GameObject FreePlanetFill;
    GameObject NoSelectionFill;
    GameObject StarFill;

    Text label;
    Text planetResources;
    Text planetCharacteristics;
    Text energy;
    Text ownerName;

    Text scoutCosts;
    Text minerCosts;
    Text warshipCosts;
    Text colonizerCosts;

    Button colonizeButton;
    Button mineButton;

    Image icon;

    // Use this for initialization
    void Start()
    {
        GameController = GameObject.Find("GameController").GetComponent<GameController>();

        transform.position += new Vector3(256, 0, 0);

        NamePanel = GameObject.Find("NamePanel");
        PlanetCharacteristicsPanel = GameObject.Find("PlanetCharacteristicsPanel");
        PlanetResourcesPanel = GameObject.Find("PlanetResourcesPanel");
        BuildPanel = GameObject.Find("BuildPanel");
        ShipPanel = GameObject.Find("ShipPanel");

        FreePlanetFill = GameObject.Find("FreePlanetFill");
        NoSelectionFill = GameObject.Find("NoSelectionFill");
        StarFill = GameObject.Find("StarFill");

        label = NamePanel.GetComponentInChildren<Text>();
        planetResources = PlanetResourcesPanel.GetComponentsInChildren<Text>().Last();
        planetCharacteristics = PlanetCharacteristicsPanel.GetComponentsInChildren<Text>().Last();
        energy = ShipPanel.GetComponentInChildren<Text>();
        colonizeButton = ShipPanel.GetComponentsInChildren<Button>().Last();
        mineButton = ShipPanel.GetComponentsInChildren<Button>().First();
        ownerName = NamePanel.GetComponentsInChildren<Text>().Last();

        scoutCosts = BuildPanel.GetComponentsInChildren<Text>()[1];
        minerCosts = BuildPanel.GetComponentsInChildren<Text>()[2];
        warshipCosts = BuildPanel.GetComponentsInChildren<Text>()[3];
        colonizerCosts = BuildPanel.GetComponentsInChildren<Text>()[4];

        icon = NamePanel.GetComponentsInChildren<Image>().Last();


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
        PlanetResourcesPanel.SetActive(false);
        BuildPanel.SetActive(false);
        FreePlanetFill.SetActive(false);
        NoSelectionFill.SetActive(false);
        StarFill.SetActive(false);

        energy.text = "Energy: " + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).GetActionPoints().ToString();

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Colonizer>() as Colonizer) != null)
        {
            colonizeButton.gameObject.SetActive(true);
        }
        else
        {
            colonizeButton.gameObject.SetActive(false);
        }


        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Miner>() as Miner) != null)
        {
            mineButton.gameObject.SetActive(true);
        }
        else
        {
            mineButton.gameObject.SetActive(false);
        }
    }

    void ShowOwnedPlanetPanels()
    {
        PlanetCharacteristicsPanel.SetActive(true);
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

        scoutCosts.text = '-' + GameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededSolarPower.ToString() +
            "     " +
            '-' + GameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededMinerals.ToString() +
            "     " +
            '-' + GameController.GetCurrentPlayer().spaceshipsCosts.scoutNeededPopulation.ToString();

        minerCosts.text = '-' + GameController.GetCurrentPlayer().spaceshipsCosts.minerNeededSolarPower.ToString() +
            "     " +
            '-' + GameController.GetCurrentPlayer().spaceshipsCosts.minerNeededMinerals.ToString() +
            "     " +
            '-' + GameController.GetCurrentPlayer().spaceshipsCosts.minerNeededPopulation.ToString();

        warshipCosts.text = '-' + GameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededSolarPower.ToString() +
            "     " +
            '-' + GameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededMinerals.ToString() +
            "     " +
            '-' + GameController.GetCurrentPlayer().spaceshipsCosts.warshipNeededPopulation.ToString();


        colonizerCosts.text = '-' + GameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededSolarPower.ToString() +
            "     " +
            '-' + GameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededMinerals.ToString() +
            "     " +
            '-' + GameController.GetCurrentPlayer().spaceshipsCosts.colonizerNeededPopulation.ToString();



    }

    void ShowFreePlanetPanels()
    {

        PlanetCharacteristicsPanel.SetActive(true);
        PlanetResourcesPanel.SetActive(true);
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

        
    }

    // Update is called once per frame
    void Update()
    {

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
            ((EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship) != null))// if Spaceship
        {
            ShowNamePanel();
            ShowShipPanel();
        }

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Star>() as Star) != null) //if Star
        {
            ShowNamePanel();
            ShowStarPanels();

        }


        if (EventManager.selectionManager.SelectedObject != null &&
            (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet) != null &&
            (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Ownable).GetOwner() == GameController.GetCurrentPlayer()) // if owned planet
        {
            ShowNamePanel();
            ShowOwnedPlanetPanels();
        }


        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet) != null &&
           (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Ownable).GetOwner() != GameController.GetCurrentPlayer()) //Free Planet
        {
            ShowNamePanel();
            ShowFreePlanetPanels();
        }
    }

    IEnumerator Show()
    {


        animating = true;
        float startTime = Time.time;
        Vector3 direction = new Vector3(-256, 0, 0);
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
        Vector3 direction = new Vector3(256, 0, 0);
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