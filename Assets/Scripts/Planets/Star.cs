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
    public GameObject mesh;

    void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>().gameObject;
        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        uiListener = GameObject.Find("Canvas").GetComponent<UIHoverListener>();
        UpdateCoordinates();
    }

    private void Start()
    {
        Debug.Log("star start: " + name);
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
    private int GetSolarPower(int solarPowerCount)
    {
        if (resources.solarPower >= solarPowerCount)
            resources.solarPower -= solarPowerCount;

        return solarPowerCount;
    }
    public bool GiveSolarPower(Player player, int solarPowerCount)
    {
        player.solarPower += (GetSolarPower(solarPowerCount));
        return true;
    }
    public int GetSolarPower()
    {
        return resources.solarPower;
    }
}
