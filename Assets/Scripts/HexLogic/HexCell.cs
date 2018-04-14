using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.HexLogic;
using System.Linq;

public class HexCell : MonoBehaviour
{
    public HexCoordinates Coordinates;
    public GameObject ObjectInCell;
    private ArrayList isVisibleByList;
    public EHexState State { get; set; }
    public Material VisibleMaterial, HiddenMaterial, UndiscoveredMaterial;

    private void Awake()
    {
        ObjectInCell = null;
        isVisibleByList = new ArrayList();
    }

    private void Start()
    {
        UnDiscover();
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

    public void Discover(Ownable owned)
    {
        isVisibleByList.Add(owned);
        State = EHexState.Visible;
        if (!IsEmpty()) ObjectInCell.SetActive(true);
        gameObject.GetComponentInChildren<MeshRenderer>().material = VisibleMaterial;

    }

    public void Hide(Ownable owned)
    {
        isVisibleByList.Remove(owned);
        if (isVisibleByList.ToArray().Length == 0)
        {
            State = EHexState.Hidden;
            if (!IsEmpty()) ObjectInCell.SetActive(false);
            gameObject.GetComponentInChildren<MeshRenderer>().material = HiddenMaterial;
        }
    }

    public void Hide()
    {
        isVisibleByList.Clear();
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
