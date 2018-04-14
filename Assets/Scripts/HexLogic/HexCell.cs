using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.HexLogic;
using System.Linq;

public class HexCell : MonoBehaviour
{
    public HexCoordinates Coordinates;
    public GameObject ObjectInCell;
    public EHexState State { get; set; }
    public Material VisibleMaterial, HiddenMaterial, UndiscoveredMaterial;

    private void Awake()
    {
        ObjectInCell = null;
    }

    private void Start()
    {
        UnDiscover();
        UpdateState();
    }

    public void AssignObject(GameObject objectInCell)
    {
        this.ObjectInCell = objectInCell;
    }

    public void ClearObject()
    {
        ObjectInCell = null;
    }

    public bool IsEmpty()
    {
        if (ObjectInCell == null) return true;
        return false;
    }

    public void UpdateState()
    {
        if (!IsEmpty() && ObjectInCell.tag == "Unit")
        {
            var spaceships = FindObjectsOfType<Spaceship>();

            foreach (Spaceship spaceship in spaceships) // Hide hexes in large proximity, I could iterate over the whole array but this might be faster
            {
                var gameObjectsInProximity =
                Physics.OverlapSphere(spaceship.transform.position, 100 /*Radius*/)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();

                var cells = gameObjectsInProximity.Where(o => o.tag == "HexCell");
                foreach (GameObject c in cells)
                {
                    if ((c.GetComponent<HexCell>() as HexCell).State == EHexState.Visible)
                    {
                        (c.GetComponent<HexCell>() as HexCell).Hide();
                    }

                }
            }
            foreach (Spaceship spaceship in spaceships) // UnhideHide hexes in radar proximity
            {

                var gameObjectsInRadar =
                Physics.OverlapSphere(spaceship.transform.position, spaceship.GetComponent<Spaceship>().RadarRange /*Radius*/)
                .Except(new[] { GetComponent<Collider>() })
                .Select(c => c.gameObject)
                .ToArray();

                var cells = gameObjectsInRadar.Where(o => o.tag == "HexCell");
                foreach (GameObject c in cells)
                {
                    (c.GetComponent<HexCell>() as HexCell).Discover();
                }
            }

        }

    }

    public void Discover()
    {
        State = EHexState.Visible;
        if (!IsEmpty()) ObjectInCell.SetActive(true);
        gameObject.GetComponentInChildren<MeshRenderer>().material = VisibleMaterial;

    }

    public void Hide()
    {
        State = EHexState.Hidden;
        if (!IsEmpty()) ObjectInCell.SetActive(false);
        gameObject.GetComponentInChildren<MeshRenderer>().material = HiddenMaterial;


    }

    public void UnDiscover()
    {
        State = EHexState.Undiscovered;
        if (!IsEmpty() && ObjectInCell.tag != "Star") ObjectInCell.SetActive(false);
        gameObject.GetComponentInChildren<MeshRenderer>().material = UndiscoveredMaterial;

        if (!IsEmpty() && ObjectInCell.tag == "Star")
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material = VisibleMaterial;
        }

    }


}
