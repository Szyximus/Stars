using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;

    public HexCell[] cells;

    public Text cellLabelPrefab;

    Canvas gridCanvas;

    HexMesh hexMesh;

    private MyUIHoverListener uiListener;


    void Awake()
    {

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
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
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }

    void Start()
    {
        hexMesh.Triangulate(cells);
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

}