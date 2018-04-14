using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

    public int chunkCountX = 10, chunkCountZ = 10;

    int cellCountX, cellCountZ;

    public HexGridChunk chunkPrefab;

    HexGridChunk[] chunks;

    public HexCell cellPrefab;

    public HexCell[] cells;

    public Text cellLabelPrefab;

    private MyUIHoverListener uiListener;


    void Awake()
    {

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
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

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

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
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void Start()
    {

        uiListener = GameObject.Find("WiPCanvas").GetComponent<MyUIHoverListener>();
    }

    void Update()
    {
        if (Input.GetButtonUp("MouseLeft"))
        {
            if (!uiListener.isUIOverride) HandleInput();
            Thread.Sleep(100);  //ugly way of not running command couple times during one click
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
        var a = cells.Where(c => c.coordinates == coordinates);
        if (a.Any()) return a.FirstOrDefault();
        else return null;
    }

    void TouchCell(Vector3 position)
    {
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        GameObject selectedObject;
        if ((selectedObject = EventManager.selectionManager.SelectedObject) != null)
            if (selectedObject.tag == "Unit")
            {
                var spaceship = selectedObject.GetComponent("Spaceship") as Spaceship;
                if (coordinates != spaceship.Coordinates && !spaceship.flying && FromCoordinates(coordinates).IsEmpty())
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

    public void SetupNewTurn(Player currentPlayer)
    {
        HideAll();
        ShowAllInRadarRange(currentPlayer);
    }

    void HideAll()
    {
        foreach (HexCell cell in cells)
        {
            cell.Hide();
        }
    }

    void ShowAllInRadarRange(Player player)
    { 
        foreach (Ownable owned in player.GetOwned())
        {
            var gameObjectsInRadar =
                Physics.OverlapSphere(owned.transform.position, owned.radarRange /*Radius*/)
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