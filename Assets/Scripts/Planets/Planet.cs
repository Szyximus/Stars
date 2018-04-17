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

public class Planet : Ownable
{
    [System.Serializable]
    public struct PlanetCharacteristics
    {
        public int Temperature;
        public int Radiation;
        public int Oxygen;
    }

    [System.Serializable]
    public struct PlanetResources
    {
        public int Minerals;
        public int Energy;
        public int Population;
    }

    public PlanetCharacteristics Characteristics;
    public PlanetResources Resources;

    private MyUIHoverListener uiListener;
    private HexGrid grid;
    public HexCoordinates Coordinates { get; set; }

    private void Awake()
    {
        RadarRange = 40f;
    }

    void Start()
    {
        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        uiListener = GameObject.Find("WiPCanvas").GetComponent<MyUIHoverListener>();

        UpdateCoordinates();
        Debug.Log("Start planet " + name + ", coordinates: " + Coordinates + " - " + transform.position);
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
            writer.WriteRawValue(this.transform.position.ToString().Substring(1, this.transform.position.ToString().Length-2));
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
}