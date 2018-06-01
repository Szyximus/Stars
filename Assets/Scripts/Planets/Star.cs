using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Star : MonoBehaviour
{
    [System.Serializable]
    public struct StarResources
    {
        public int solarPower;
    }

    StarResources starResources;

    private HexGrid grid;
    public HexCoordinates Coordinates { get; set; }
    private UIHoverListener uiListener;
    Vector3 targetMenuPosition;
    // Use this for initialization
    void Start()
    {
        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        UpdateCoordinates();
        uiListener = GameObject.Find("Canvas").GetComponent<UIHoverListener>();
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
        if (starResources.solarPower >= solarPowerCount)
            starResources.solarPower -= solarPowerCount;

        return solarPowerCount;
    }
    public bool GiveSolarPower(Player player, int solarPowerCount)
    {
        player.solarPower += (GetSolarPower(solarPowerCount));
        return true;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && isActiveAndEnabled &&
            EventManager.selectionManager.SelectedObject != null &&
            EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship != null)
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

}
