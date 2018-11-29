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
using Assets.Scripts.HexLogic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

//represents an entire grid of hexes, made of 10x10 chunks
//Based on: https://catlikecoding.com/unity/tutorials/hex-map/part-1/ 
public class HexGrid : MonoBehaviour
{

    public int ChunkCountX = 10, ChunkCountZ = 10;

    int cellCountX, cellCountZ;

    public HexGridChunk ChunkPrefab;

    HexGridChunk[] chunks;

    public HexCell CellPrefab;

    public HexCell[] cells;

    private UIHoverListener uiListener;

    private List<HexCell> path;

    private bool isPath = false;


    void Awake()
    {

        cellCountX = ChunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = ChunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();

        uiListener = GameObject.Find("Canvas").GetComponent<UIHoverListener>();
        path = new List<HexCell>();
        isPath = false;
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(CellPrefab);
        cell.transform.localPosition = position;
        cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        //Instantiate labels for debug:

        //Text label = Instantiate<Text>(cellLabelPrefab);
        ////label.rectTransform.SetParent(gridCanvas.transform, false);
        //label.rectTransform.anchoredPosition =
        //    new Vector2(position.x, position.z);
        //label.text = cell.coordinates.ToStringOnSeparateLines();

        AddCellToChunk(x, z, cell);

    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * ChunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[ChunkCountX * ChunkCountZ];

        for (int z = 0, i = 0; z < ChunkCountZ; z++)
        {
            for (int x = 0; x < ChunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(ChunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonUp("MouseLeft"))
        {
            if (!uiListener.IsUIOverride) HandleInput();
            Thread.Sleep(100);  //ugly way of not running command couple times during one click
        }

        if (Input.GetButtonUp("MouseRight"))
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                if (EventManager.selectionManager.SelectedObject != null && hit.collider.gameObject.GetComponent<Ownable>() == null &&
                   EventManager.selectionManager.TargetObject == null && hit.collider.gameObject.GetComponent<Star>() == null)
                {
                    EventManager.selectionManager.SelectedObject = null;
                }
                if (EventManager.selectionManager.TargetObject != null && hit.collider.gameObject.GetComponent<Ownable>() == null &&
                     hit.collider.gameObject.GetComponent<Star>() == null)
                {
                    EventManager.selectionManager.TargetObject = null;
                }

            }
            Thread.Sleep(100);

        }

        if( !FlyingSpaceshipLock )
            PathToDrawOnMouseHover( DebugOutputDrawingPath );
    }

    /* Captures hovered by mouse hex coordinates, optimization trick to calculate path only when hovered hex changes */
    private HexCoordinates MouseHooverCoordinates;

    /* Contains HexCells on calculated path to draw */
    private List<HexCell> PathToDraw;

    /* Created to not calculate paths when during spaceship flight */
    public static bool FlyingSpaceshipLock = false;

    /* Calculates path on change of hovered cell in coroutine, places it in 
     * PathToDraw field and after that executes action given as argument  */
    private void PathToDrawOnMouseHover( System.Action action )
    {
        if ((EventManager.selectionManager.SelectedObject) == null || (EventManager.selectionManager.SelectedObject.tag) != "Unit")
            return;

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCoordinates lastCoordinates = MouseHooverCoordinates;
            MouseHooverCoordinates = HexCoordinates.FromPosition(hit.point);

            if (lastCoordinates != MouseHooverCoordinates)
            {
                StartCoroutine( GetPathToDraw() );

                action();
            }
        }
    }

    /* Calculates path from selected spaceship to hovered cell */
    private IEnumerator<int> GetPathToDraw()
    {
        GameObject selectedObject;
        if ((selectedObject = EventManager.selectionManager.SelectedObject) != null)
            if (selectedObject.tag == "Unit")
            {
                var spaceship = selectedObject.GetComponent<Spaceship>();
                if (MouseHooverCoordinates != spaceship.Coordinates && !spaceship.Flying && FromCoordinates(MouseHooverCoordinates) != null && FromCoordinates(MouseHooverCoordinates).IsEmpty())
                {
                    PathToDraw = Pathfinder.CalculatePath( this.FromCoordinates(spaceship.Coordinates) , this.FromCoordinates(MouseHooverCoordinates) );
                }
            }

        yield return 1;
    }

    /* Example debug action, to be replaced by drawing action or sth */
    private void DebugOutputDrawingPath()
    {
        if ( PathToDraw == null )
            return;

        string pathString = " Trasa: ";
        foreach (var x in PathToDraw )
        {
            pathString += x.Coordinates.ToString();
            pathString += ",";
        }
        Debug.Log(pathString);
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            TouchCell(hit.point);
        }
    }

    public HexCell FromCoordinates(HexCoordinates coordinates)
    {
        var a = cells.Where(c => c.Coordinates == coordinates);
        if (a.Any()) return a.FirstOrDefault();
        else return null;
    }

    public HexCell FromCoordinates(int x, int z)
    {
        var a = cells.Where(c => c.Coordinates.X == x && c.Coordinates.Z == z);
        if (a.Any())
            return a.FirstOrDefault();
        else
            return null;
    }

    public void TouchCell(Vector3 position)

    {
        if (HexCoordinates.FromPosition(position) != null)
        {
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);

            GameObject selectedObject;
            if ((selectedObject = EventManager.selectionManager.SelectedObject) != null)
                if (selectedObject.tag == "Unit")
                {
                    var spaceship = selectedObject.GetComponent<Spaceship>();
                    if (coordinates != spaceship.Coordinates && !spaceship.Flying && FromCoordinates(coordinates) != null && FromCoordinates(coordinates).IsEmpty())
                    {
                        spaceship.Destination = coordinates;
                        //DEBUG - after mouse clik unit goes {speed} fields in destination direction, hold mouse down to "see path"
                        //path = Pathfinder.CalculatePath(this.FromCoordinates(EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>().Coordinates), this.FromCoordinates(spaceship.Destination));
                        StartCoroutine(PathInBackground(this.FromCoordinates(EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>().Coordinates), this.FromCoordinates(spaceship.Destination)));
                        if (isPath) StartCoroutine(spaceship.MoveTo(spaceship.Destination, path));
                    }
                }
            if (FromCoordinates(coordinates) != null) EventManager.selectionManager.GridCellSelection =
                FromCoordinates(coordinates); //it's only one match, First() used to change type
                                              //Debug.Log("touched at " + coordinates);
        }


    }

    public IEnumerator PathInBackground(HexCell dest, HexCell start)
    {
        path.Clear();
        isPath = false;
        path = Pathfinder.CalculatePath(dest, start);
        isPath = true;
        yield return 1;
    }

    public void SetupNewTurn(Player currentPlayer)
    {
        HideOrUnDiscoverAll(currentPlayer);
        ShowAllInRadarRange(currentPlayer);
    }

    void HideOrUnDiscoverAll(Player currentPlayer)
    {
        foreach (HexCell cell in cells)
        {
            if (cell.IsDiscoveredBy(currentPlayer))
            {
                cell.Hide();
            }
            else
            {
                cell.UnDiscover();
            }
        }
    }

    HexCell[] CellsInRange(Vector3 position, float radarRange)
    {
        if (radarRange == 0)
            return new HexCell[0];
        return Physics.OverlapSphere(position, radarRange /*Radius*/)
            .Except(new[] { GetComponent<Collider>() })
            .Where(o => o.tag == "HexCell")
            .Select(c => c.gameObject.GetComponent<HexCell>())
            .ToArray();
    }

    public void UpdateInRadarRange(Ownable owned, Vector3 oldPosition)
    {
        var cellsInOldRadar = CellsInRange(oldPosition, owned.RadarRange);
        var cellsInRadar = CellsInRange(owned.transform.position, owned.RadarRange);

        foreach (HexCell cell in cellsInOldRadar.Except(cellsInRadar))
        {
            cell.Hide(owned);
        }

        foreach (HexCell cell in cellsInRadar.Except(cellsInOldRadar))
        {
            cell.Discover(owned);
        }
    }

    public void ShowAllInRadarRange(Ownable owned)
    {

        var cellsInRange = CellsInRange(owned.transform.position, owned.RadarRange);
        foreach (HexCell cell in cellsInRange)
        {
            cell.Discover(owned);
        }
    }

    void ShowAllInRadarRange(Player player)
    {
        foreach (Ownable owned in player.GetOwned())
        {
            ShowAllInRadarRange(owned);
        }
    }
}
