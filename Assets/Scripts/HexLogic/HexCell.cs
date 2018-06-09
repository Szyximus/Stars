/*
    Copyright (c) 2018, Szymon Jakóbczyk, Paweł Płatek, Michał Mielus, Maciej Rajs, Minh Nhật Trịnh, Izabela Musztyfaga
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation 
          and/or other materials provided with the distribution.
        * Neither the name of the [organization] nor the names of its contributors may be used to endorse or promote products derived from this software 
          without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
    USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

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
