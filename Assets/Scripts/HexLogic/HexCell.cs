using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {
    public HexCoordinates coordinates;
    public GameObject Object;

        private void Awake()
    {
        Object = null;
    }

    public void AssignObject(GameObject _Object)
    {
        Object = _Object;
    }

    public void ClearObject()
    {
        Object = null;
    }

    public bool isEmpty()
    {
        if (Object == null) return true;
        return false;
    }


}
