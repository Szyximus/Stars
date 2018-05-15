using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class SideMenu : MonoBehaviour
{
    bool shown, animating = false;
    RectTransform rectTransform;
    Text label;
    Text energy;
    public Text OwnerName;
    public Text planetResources;
    public Text planetCharacteristics;
    Button button;
    Button colonizeButton;
    Button buildColonizerButton;
    Button buildScoutButton;
    Button buildMinerButton;
    Button buildWarshipButton;
    Button mineButton;

    // Use this for initialization
    void Start()
    {
        //RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        transform.position += new Vector3(170, 0, 0);
        label = GameObject.Find("Name").GetComponent<Text>();
        planetResources = GameObject.Find("PlanetResources").GetComponent<Text>();
        planetCharacteristics = GameObject.Find("PlanetCharacteristics").GetComponent<Text>();
        energy = GameObject.Find("Energy").GetComponent<Text>();
        colonizeButton = GameObject.Find("ColonizeButton").GetComponent<Button>();
        mineButton = GameObject.Find("MineButton").GetComponent<Button>();
        buildColonizerButton = GameObject.Find("BuildColonizerButton").GetComponent<Button>();
        buildScoutButton = GameObject.Find("BuildScoutButton").GetComponent<Button>();
        buildMinerButton = GameObject.Find("BuildMinerButton").GetComponent<Button>();
        buildWarshipButton = GameObject.Find("BuildWarshipButton").GetComponent<Button>();
        OwnerName = GameObject.Find("OwnerName").GetComponent<Text>();


    }

    // Update is called once per frame
    void Update()
    {

        if (EventManager.selectionManager.SelectedObject == null && shown && !animating)
        {
            StartCoroutine(Hide());
        }

        if (EventManager.selectionManager.SelectedObject != null && !shown && !animating)
        {
            StartCoroutine(Show());
        }
        if (shown && EventManager.selectionManager.SelectedObject != null) label.text = EventManager.selectionManager.SelectedObject.name.Replace("(Clone)", "");

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Ownable>() as Ownable) != null)
        {
            string _owner = (EventManager.selectionManager.SelectedObject.GetComponent<Ownable>() as Ownable).GetOwnerName();

            if (_owner == "")
                OwnerName.text = "No Owner";
            else
                OwnerName.text = "Owner: " + _owner;
            OwnerName.gameObject.SetActive(true);
        }
        else
        {
            OwnerName.gameObject.SetActive(false);
        }

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Colonizer>() as Colonizer) != null)
        {
            colonizeButton.gameObject.SetActive(true);
        }
        else
        {
            colonizeButton.gameObject.SetActive(false);
        }

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship) != null)
        {
            energy.gameObject.SetActive(true);
            energy.text = "Energy: " + (EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship).GetActionPoints().ToString();
        }
        else
        {
            energy.gameObject.SetActive(false);
        }
        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Miner>() as Miner) != null)
        {
            mineButton.gameObject.SetActive(true);
        }
        else
        {
            mineButton.gameObject.SetActive(false);
        }

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet) != null && (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Ownable).GetOwner() == GameController.GetCurrentPlayer())
        {
            buildColonizerButton.gameObject.SetActive(true);
            buildScoutButton.gameObject.SetActive(true);
            buildMinerButton.gameObject.SetActive(true);
            buildWarshipButton.gameObject.SetActive(true);
            planetCharacteristics.gameObject.SetActive(true);
            Planet planet = EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet;
            planetCharacteristics.text = ("Temperature: " + planet.characteristics.temperature.ToString() + "\n" +
                                         "Oxygen: " + planet.characteristics.oxygen.ToString() + "\n" +
                                         "Radiation: " + planet.characteristics.radiation.ToString() + "\n" +
                                         "HealthPoints: " + planet.characteristics.healthPoints.ToString()).Replace("\n", System.Environment.NewLine);
        }
        else
        {
            buildColonizerButton.gameObject.SetActive(false);
            buildScoutButton.gameObject.SetActive(false);
            buildMinerButton.gameObject.SetActive(false);
            buildWarshipButton.gameObject.SetActive(false);
        }
        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet) != null &&
           (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Ownable).GetOwner() != GameController.GetCurrentPlayer())
        {
            planetCharacteristics.gameObject.SetActive(true);
            planetResources.gameObject.SetActive(true);
            Planet planet = EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet;
            planetCharacteristics.text = ("Temperature: " + planet.characteristics.temperature.ToString() + "\n" +
                                         "Oxygen: " + planet.characteristics.oxygen.ToString() + "\n" +
                                         "Radiation: " + planet.characteristics.radiation.ToString() + "\n" +
                                         "Habitability: " + planet.characteristics.habitability.ToString() + "\n" +
                                         "HealthPoints: " + planet.characteristics.healthPoints.ToString()).Replace("\n", System.Environment.NewLine);
            planetResources.text = "Minerals: " + planet.resources.minerals.ToString();
        }
        else
        {
            planetResources.gameObject.SetActive(false);
        }
        //hack// jak jest zaznaczony statek to rzuca nullami, trzeba to jakos ladnie obsluzyc ale teraz nie ma czasu
        try
        {
            if (EventManager.selectionManager.SelectedObject.GetComponent<Planet>() as Planet == null)
            {
                planetCharacteristics.gameObject.SetActive(false);
            }
        }
        catch (System.Exception e)
        {

        }

    }

    IEnumerator Show()
    {


        animating = true;
        float startTime = Time.time;
        Vector3 direction = new Vector3(-170, 0, 0);
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
        Vector3 direction = new Vector3(170, 0, 0);
        var endPos = transform.position + direction;

        while (Time.time - startTime < 0.25) //the movement takes exactly 0,25 s. regardless of framerate
        {

            transform.position += direction * Time.deltaTime * 4;
            yield return null;
        }
        transform.position = endPos;
        label.text = " ";
        shown = false;
        animating = false;
    }
}