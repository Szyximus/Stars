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
using UnityEngine.EventSystems;
using UnityEngine;
using Assets.Scripts;
using System.Linq;


public class PathDrawer : MonoBehaviour
{
    PathDrawer main;
    Vector3 offset = new Vector3(0, 0.02f, 0);

    private Renderer renderer;

    public Material LineMat; //Material of the camera trapezoid

    private LineRenderer line;

    public GameController gameController;

    private HexGrid grid;

    void Awake()
    {
        
        main = this;
        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        line = this.gameObject.GetComponent<LineRenderer>();
    }


        // Use this for initialization
        void Start()
    {

        renderer = GetComponent<Renderer>();
        //transform.position = new Vector3(-1000, -1000, -1000);
        // Add a Line Renderer to the GameObject

        // Set the width of the Line Renderer
        //line.startWidth = 0.5f;
       // line.endWidth = 0.0f;
        // Set the number of vertex fo the Line Renderer
        line.positionCount = 0;
        //line.material = LineMat;
        transform.position = new Vector3(-1000, -1000, -1000);

    }

    // Update is called once per frame
    void Update()
    {
        if (EventManager.selectionManager.SelectedObject != null && grid.PathToDraw != null)
        {
            int i = 1;
            
            line.positionCount = grid.PathToDraw.Count + 1;
            transform.position = grid.PathToDraw.Last().transform.position + offset;
            renderer.material.color = gameController.GetCurrentPlayer().color;
            line.material.color = gameController.GetCurrentPlayer().color;
            Vector3 start = Vector3.Lerp(EventManager.selectionManager.SelectedObject.transform.position + offset, grid.PathToDraw.First().transform.position + offset, 0.5f);
            line.SetPosition(0, start); 

            foreach (HexCell cell in grid.PathToDraw){
                line.SetPosition(i, cell.transform.position + offset);
                i++;
            }

            Vector3 end = Vector3.Lerp(line.GetPosition(i-2), grid.PathToDraw.Last().transform.position + offset, 0.5f);
            line.SetPosition(i-1, end);


        }
        else
        {
            transform.position = new Vector3(-1000, -1000, -1000);
            line.positionCount = 0;
        }


        //if (EventManager.selectionManager.SelectedObject != null)
        //{
            
        //    if (Vector3.Distance(transform.position , EventManager.selectionManager.SelectedObject.transform.position + offset) > 1f) 
        //    {
        //        EventManager.selectionManager.TargetObject = null;
        //    }

        //    if (transform.position != EventManager.selectionManager.SelectedObject.transform.position + offset)
        //    {
        //        transform.position = EventManager.selectionManager.SelectedObject.transform.position + offset;
        //        renderer.material.color  = gameController.GetCurrentPlayer().color;
        //    }

        //}
        //else
        //{
        //    if (transform.position != new Vector3(-1000, -1000, -1000))
        //    {
        //        transform.position = new Vector3(-1000, -1000, -1000);
        //        sound.Play();
        //        EventManager.selectionManager.TargetObject = null;

        //    }
        //    EventManager.selectionManager.TargetObject = null;

        //}

        ////if (transform.hasChanged)
        ////{
        ////    EventManager.selectionManager.TargetObject = null;
        ////    sound.Play();
        ////    transform.hasChanged = false;
        ////}

        //transform.Rotate(Vector3.up * 20 * Time.deltaTime);
        ////transform.hasChanged = false;

    }

}
