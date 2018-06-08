using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

//represents an entire grid of hexes, made of 10x10 chunks
public class HexGrid : MonoBehaviour
{

    public int ChunkCountX = 10, ChunkCountZ = 10;

    int cellCountX, cellCountZ;

    public HexGridChunk ChunkPrefab;

    HexGridChunk[] chunks;

    public HexCell CellPrefab;

    public HexCell[] cells;

    private UIHoverListener uiListener;


    void Awake()
    {

        cellCountX = ChunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = ChunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();

        uiListener = GameObject.Find("Canvas").GetComponent<UIHoverListener>();
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

    void TouchCell(Vector3 position)

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
                        StartCoroutine(spaceship.MoveTo(spaceship.Destination));
                    }
                }
            if (FromCoordinates(coordinates) != null) EventManager.selectionManager.GridCellSelection =
                FromCoordinates(coordinates); //it's only one match, First() used to change type
                                              //Debug.Log("touched at " + coordinates);
        }


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