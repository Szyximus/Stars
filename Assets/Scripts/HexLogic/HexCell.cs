using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.HexLogic;
using System.Linq;

//represents a single hexagonal field on a map
public class HexCell : MonoBehaviour
{
    public HexCoordinates Coordinates;
    public GameObject ObjectInCell;
    private ArrayList visibleByList;
    private HashSet<Player> discoveredBy;
    public EHexState State { get; set; }
    public Material VisibleMaterial, HiddenMaterial, UndiscoveredMaterial;

    private void Awake()
    {
        ObjectInCell = null;
        visibleByList = new ArrayList();
        discoveredBy = new HashSet<Player>();

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

    public bool IsDiscoveredBy(Player player)
    {
        if (discoveredBy.Contains(player))
        {
            return true;
        }
        return false;
    }

    public void Discover(Ownable owned)
    {
        visibleByList.Add(owned);
        discoveredBy.Add(owned.GetOwner());

        State = EHexState.Visible;
        if (!IsEmpty()) ObjectInCell.SetActive(true);
        gameObject.GetComponentInChildren<MeshRenderer>().material = VisibleMaterial;

    }

    public void Hide(Ownable owned)
    {
        visibleByList.Remove(owned);
        if (visibleByList.Count == 0)
        {
            State = EHexState.Hidden;
            gameObject.GetComponentInChildren<MeshRenderer>().material = HiddenMaterial;

            if (!IsEmpty())
            {
                if(ObjectInCell.tag != "Star" && ObjectInCell.tag != "Planet")
                    ObjectInCell.SetActive(false);
                if(ObjectInCell.tag == "Star")
                    gameObject.GetComponentInChildren<MeshRenderer>().material = VisibleMaterial;
            }
        }
    }

    public void Hide()
    {
        visibleByList.Clear();
        State = EHexState.Hidden;
        gameObject.GetComponentInChildren<MeshRenderer>().material = HiddenMaterial;

        if (!IsEmpty())
        {
            if (ObjectInCell.tag != "Star" && ObjectInCell.tag != "Planet")
                ObjectInCell.SetActive(false);
            if (ObjectInCell.tag == "Star")
                gameObject.GetComponentInChildren<MeshRenderer>().material = VisibleMaterial;
        }
    }

    public void UnDiscover()
    {
        visibleByList.Clear();
        State = EHexState.Undiscovered;

        if (!IsEmpty() && ObjectInCell.tag != "Star")
        {
            ObjectInCell.SetActive(false);
        }

        gameObject.GetComponentInChildren<MeshRenderer>().material = UndiscoveredMaterial;
        if (!IsEmpty() && ObjectInCell.tag == "Star")
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material = VisibleMaterial;
        }
    }
}
