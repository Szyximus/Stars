using Assets.Scripts;
using Assets.Scripts.HexLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Spaceship : MonoBehaviour
{

    HexGrid grid;
    public HexCoordinates Coordinates { get; set; }

    public int Speed = 5;

    public HexCoordinates Destination { get; set; }

    private MyUIHoverListener uiListener;
    public bool flying;
    public float RadarRange = 20f;

    int i = 0; //for the movement test, remove later

    // Use this for initialization
    void Start()
    {

        flying = false;


        grid = (GameObject.Find("HexGrid").GetComponent("HexGrid") as HexGrid);

        StartCoroutine(DelayedUpdate()); //Need to update coordinates after Hexes initialization is finished

        uiListener = GameObject.Find("WiPCanvas").GetComponent<MyUIHoverListener>();
    }

    void UpdateCoordinates()
    {
        Coordinates = HexCoordinates.FromPosition(gameObject.transform.position);
        if (grid.FromCoordinates(Coordinates) != null) transform.position = grid.FromCoordinates(Coordinates).transform.localPosition; //Snap object to hex
        if (grid.FromCoordinates(Coordinates) != null) grid.FromCoordinates(Coordinates).AssignObject(this.gameObject);

        if (grid.FromCoordinates(Coordinates) != null) grid.FromCoordinates(Coordinates).UpdateState();

    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnMouseUpAsButton()
    {
        if (!uiListener.isUIOverride) EventManager.selectionManager.SelectedObject = this.gameObject;

    }

    public void Move(EDirection direction)
    {
        var r = HexMetrics.innerRadius;
        var r_sqrt3 = r * 1.7320508757f;

        switch (direction)
        {
            case EDirection.TopRight:
                StartCoroutine(SmoothFly(new Vector3(r, 0, r_sqrt3))); // OLD: transform.Translate(r, 0, r_sqrt3, Space.Self)
                break;
            case EDirection.Right:
                StartCoroutine(SmoothFly(new Vector3(2 * r, 0, 0)));
                break;
            case EDirection.BottomRight:
                StartCoroutine(SmoothFly(new Vector3(r, 0, -r_sqrt3)));
                break;
            case EDirection.BottomLeft:
                StartCoroutine(SmoothFly(new Vector3(-r, 0, -r_sqrt3)));
                break;
            case EDirection.Left:
                StartCoroutine(SmoothFly(new Vector3(-2 * r, 0, 0)));
                break;
            case EDirection.TopLeft:
                StartCoroutine(SmoothFly(new Vector3(-r, 0, r_sqrt3)));
                break;
        }

    }

    IEnumerator SmoothFly(Vector3 direction)
    {
        if (grid.FromCoordinates(Coordinates) != null) grid.FromCoordinates(Coordinates).ClearObject();
        float startime = Time.time;

        Vector3 start_pos = transform.position; //Starting position.
        Vector3 end_pos = transform.position + direction; //Ending position.
        var model = GetComponentInChildren<Transform>().Find("Mesh"); //mesh component of a prefab

        while (Time.time - startime < 1) //the movement takes exactly 1 s. regardless of framerate
        {

            transform.position += direction * Time.deltaTime;
            model.transform.forward = Vector3.Lerp(model.transform.forward, direction, Time.deltaTime);
            yield return null;
        }
        model.transform.forward = direction;
        //transform.position = end_pos;
        UpdateCoordinates();

    }
    /*
     * TODO: Probably this function will be called from some round update 
     */
    public IEnumerator MoveTo(HexCoordinates dest)
    {
        flying = true;
        //while (Coordinates != dest)
        for (int i = Speed; i > 0; i--)
        {
            if (dest.Z > Coordinates.Z && dest.X >= Coordinates.X)
                Move(EDirection.TopRight);
            else if (dest.Z > Coordinates.Z && dest.X < Coordinates.X)
                Move(EDirection.TopLeft);
            else if (dest.Z < Coordinates.Z && dest.X > Coordinates.X)
                Move(EDirection.BottomRight);
            else if (dest.Z < Coordinates.Z && dest.X <= Coordinates.X)
                Move(EDirection.BottomLeft);
            else if (dest.X > Coordinates.X)
                Move(EDirection.Right);
            else if (dest.X < Coordinates.X)
                Move(EDirection.Left);
            yield return new WaitForSeconds(1.05f);

        }
        flying = false;

    }

    IEnumerator DelayedUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateCoordinates();
    }

    public void DoTestStuff()
    {
        if (EventManager.selectionManager.SelectedObject.tag == "Unit")
        {
            (EventManager.selectionManager.SelectedObject.GetComponent("Spaceship") as Spaceship).Move((EDirection)i);
            i++;
            if (i > 5) i = 0;

            Debug.Log(string.Format("Destination: {0}", Destination));
        }
    }
}


