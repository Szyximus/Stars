using Assets.Scripts;
using Assets.Scripts.HexLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Spaceship : Ownable
{

    private HexGrid grid;
    private ParticleSystem burster;
    private Light bursterLight;
    public HexCoordinates Coordinates { get; set; }
    private Vector3 oldPosition;
    public HexCoordinates Destination { get; set; }

    private MyUIHoverListener uiListener;
    private AudioSource engineSound;

    public bool Flying;
    public int MaxActionPoints;
    private int actionPoints;

    int i = 0; //for the movement test, remove later
    private bool initialized = false;

    private void Awake()
    {
        Flying = false;
        RadarRange = 26f;
        MaxActionPoints = 7;
    }

    void Start()
    {
        if(!initialized)
            Init();
    }

    public void Init()
    {
        initialized = true;
        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        // StartCoroutine(DelayedUpdate()); //Need to update coordinates after Hexes initialization is finished
        UpdateCoordinates();
        uiListener = GameObject.Find("WiPCanvas").GetComponent<MyUIHoverListener>();
        burster = gameObject.GetComponentInChildren<ParticleSystem>();
        bursterLight = gameObject.GetComponentInChildren<Light>();
        engineSound = gameObject.GetComponent<AudioSource>();

        TurnEnginesOff();
    }

    override
    public void SetupNewTurn()
    {
        actionPoints = MaxActionPoints;
    }

    void UpdateCoordinates()
    {
        Coordinates = HexCoordinates.FromPosition(gameObject.transform.position);
        if (grid.FromCoordinates(Coordinates) != null) transform.position = grid.FromCoordinates(Coordinates).transform.localPosition; //Snap object to hex
        if (grid.FromCoordinates(Coordinates) != null)
        {
            grid.FromCoordinates(Coordinates).AssignObject(this.gameObject);
        }
        grid.UpdateInRadarRange(this, oldPosition);
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnMouseUpAsButton()
    {
        if (!uiListener.IsUIOverride && isActiveAndEnabled) EventManager.selectionManager.SelectedObject = this.gameObject;
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
        oldPosition = this.transform.position;

        if (grid.FromCoordinates(Coordinates) != null) grid.FromCoordinates(Coordinates).ClearObject();
        float startime = Time.time;

        Vector3 start_pos = transform.position; //Starting position.
        var model = GetComponentInChildren<Transform>().Find("Mesh"); //mesh component of a prefab

        while (Time.time - startime < 1) //the movement takes exactly 1 s. regardless of framerate
        {

            transform.position += direction * Time.deltaTime;
            model.transform.forward = Vector3.Lerp(model.transform.forward, direction, Time.deltaTime * 0.5f);
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
        Flying = true;
        TurnEnginesOn();
        while (Coordinates != dest && actionPoints > 0)
        {
            Debug.Log("moving " + actionPoints);
            actionPoints--;
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
        Flying = false;
        TurnEnginesOff();
        Debug.Log("Flying done, ActionPoints: " + actionPoints);
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
            EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>().Move((EDirection)i);
            i++;
            if (i > 5) i = 0;

            Debug.Log(string.Format("Destination: {0}", Destination));
        }
    }

    private void TurnEnginesOff()
    {
        if (burster != null )
        {
            bursterLight.enabled = false;
            burster.enableEmission = false;
        }

    }

    private void TurnEnginesOn()
    {
        if (burster != null && actionPoints != 0)
        {
            bursterLight.enabled = true;
            burster.enableEmission = true;
        }

        if (engineSound != null && actionPoints != 0)
        {
            engineSound.Play();
        }
    }
}


