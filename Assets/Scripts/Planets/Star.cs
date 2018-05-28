using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

[System.Serializable]
public class Star : MonoBehaviour
{
    [System.Serializable]
    public struct StarResources
    {
        public int solarPower;
    }

    public StarResources resources;

    private HexGrid grid;
    public HexCoordinates Coordinates { get; set; }
    private UIHoverListener uiListener;

    void Awake()
    {
        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        uiListener = GameObject.Find("Canvas").GetComponent<UIHoverListener>();
        UpdateCoordinates();
    }

    private void Start()
    {
        Debug.Log("star start: " + name);
        Debug.Log(JsonUtility.ToJson(this));
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

    private void OnMouseDown()
    {
        if (!uiListener.IsUIOverride) EventManager.selectionManager.SelectedObject = this.gameObject;
    }

    private int GetSolarPower(int solarPowerCount)
    {
        if (resources.solarPower >= solarPowerCount)
            resources.solarPower -= solarPowerCount;

        return solarPowerCount;
    }
    public bool GiveSolarPower(Player player,int solarPowerCount)
    {
        player.solarPower += (GetSolarPower(solarPowerCount));
        return true;
    }
}
