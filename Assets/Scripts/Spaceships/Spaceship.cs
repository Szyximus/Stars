using Assets.Scripts;
using Assets.Scripts.HexLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

[System.Serializable]
public class Spaceship : Ownable
{
    private List<HexCell> path;
    private HexGrid grid;
    private ParticleSystem burster;
    private Light bursterLight;
    public HexCoordinates Coordinates { get; set; }
    private Vector3 oldPosition;
    public HexCoordinates Destination { get; set; }

    private UIHoverListener uiListener;
    private AudioSource engineSound;

    protected string model;

    public int neededMinerals;
    public int neededPopulation;
    public int neededSolarPower;

    [System.Serializable]
    public struct SpaceshipStatistics
    {
        public int healthPoints;
        public int attack;
        public int defense;
        public int speed;
    }

    public SpaceshipStatistics spaceshipStatistics;

    public bool Flying;
    public int MaxActionPoints;
    public int actionPoints;

    public Planet planetToAttack;
    public Spaceship spaceshipsToAttack;

    public int maxHealthPoints;

    protected new void Awake()
    {
        base.Awake();
        model = null;
        Flying = false;
        RadarRange = 26f;
        MaxActionPoints = 7;
        maxHealthPoints = spaceshipStatistics.healthPoints;

        grid = (GameObject.Find("HexGrid").GetComponent<HexGrid>());
        UpdateCoordinates();
        uiListener = GameObject.Find("Canvas").GetComponent<UIHoverListener>();
        burster = gameObject.GetComponentsInChildren<ParticleSystem>().Last();
        bursterLight = gameObject.GetComponentInChildren<Light>();
        engineSound = gameObject.GetComponent<AudioSource>();

        TurnEnginesOff();

        Debug.Log("Awake spaceship");
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
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && isActiveAndEnabled &&
            EventManager.selectionManager.SelectedObject != null &&
            EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>() as Spaceship != null &&
            EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>().GetOwner() != this.GetOwner())
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

    public void Move(HexCell destination)
    {
        if (!CanMakeAction())
            return;
        var dx = Coordinates.X - destination.Coordinates.X;
        var dz = Coordinates.Z - destination.Coordinates.Z;

        if ((dx == 0) && (dz == -1))
            Move(EDirection.TopRight);
        else if ((dx == -1) && (dz == 0))
            Move(EDirection.Right);
        else if ((dx == -1) && (dz == 1))
            Move(EDirection.BottomRight);
        else if ((dx == 0) && (dz == 1))
            Move(EDirection.BottomLeft);
        else if ((dx == 1) && (dz == 0))
            Move(EDirection.Left);
        else if ((dx == 1) && (dz == -1))
            Move(EDirection.TopLeft);
        else
            Debug.Log("Zjebales cos dx=" + dx + "  dy=" + dz);
    }

    public void Move(EDirection direction)
    {
        if (!CanMakeAction())
            return;

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
        if (CanMakeAction())
        {
            gameController.LockInput();
            TurnEnginesOn();
            //while (Coordinates != dest && actionPoints > 0)
            //{
            //    Debug.Log("moving " + actionPoints);
            //    actionPoints--;
            //    if (dest.Z > Coordinates.Z && dest.X >= Coordinates.X)
            //        Move(EDirection.TopRight);
            //    else if (dest.Z > Coordinates.Z && dest.X < Coordinates.X)
            //        Move(EDirection.TopLeft);
            //    else if (dest.Z < Coordinates.Z && dest.X > Coordinates.X)
            //        Move(EDirection.BottomRight);
            //    else if (dest.Z < Coordinates.Z && dest.X <= Coordinates.X)
            //        Move(EDirection.BottomLeft);
            //    else if (dest.X > Coordinates.X)
            //        Move(EDirection.Right);
            //    else if (dest.X < Coordinates.X)
            //        Move(EDirection.Left);
            //    yield return new WaitForSeconds(1.05f);

            //}

            path = Pathfinder.CalculatePath(grid.FromCoordinates(Coordinates), grid.FromCoordinates(dest));
            while (Coordinates != dest && actionPoints > 0)
            {
                Move(path.First());
                path.RemoveAt(0);
                actionPoints--;
                yield return new WaitForSeconds(1.05f);
            }
            Flying = false;
            TurnEnginesOff();
            Debug.Log("Flying done, ActionPoints: " + actionPoints);
            gameController.UnlockInput();
        }
    }



    IEnumerator DelayedUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateCoordinates();
    }


    public int GetActionPoints()
    {
        return actionPoints;
    }
    public void SetActionPoints(int actionPoints)
    {
        this.actionPoints += actionPoints;
    }

    public bool Attack(Ownable target)
    {
        if (EventManager.selectionManager.TargetObject != null)
        {
            Debug.Log("weszlo");
            if (target == null || target.GetOwner() == EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>().GetOwner())
            {
                Debug.Log("Cannot find planet or planet belong to you");
                return false;
            }
            else if (target != null && target.GetOwner() != EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>().GetOwner() && target.GetComponent<Spaceship>() != null)
            {
                if (GetActionPoints() > 0)
                {
                 //   GameObject SourceFire = Instantiate(GameController.AttackPrefab, transform.position, transform.rotation);
                 //   GameObject TargetFire = Instantiate(GameController.HitPrefab, target.transform.position, target.transform.rotation);
                    target.GetComponent<Spaceship>().AddHealthPoints(-this.spaceshipStatistics.attack);
                 //   Destroy(SourceFire, 1f);
                 //   Destroy(TargetFire, 1f);
                 //   Destroy(TargetFire, 1f);
                    return true;
                }
                Debug.Log("You dont have enough movement points");
                return false;
            }
            else if (target != null || target.GetOwner() != EventManager.selectionManager.SelectedObject.GetComponent<Spaceship>().GetOwner()
                && target.GetComponent<Planet>() != null)
                if (GetActionPoints() > 0)
                {
                    target.GetComponent<Planet>().AddHealthPoints(-this.spaceshipStatistics.attack);
                    return true;
                }
            Debug.Log("You dont have enough movement points");
            return false;
        }

        return false;
    }



    private void AddHealthPoints(int healthPoints)
    {
        if ((this.spaceshipStatistics.healthPoints += healthPoints) <= 0)
        {
            GameObject Explosion = Instantiate(gameController.gameApp.ExplosionPrefab, transform.position, transform.rotation);
            this.GetOwner().Lose(this);
            grid.FromCoordinates(this.Coordinates).ClearObject();
            gameController.GetCurrentPlayer().Lose(this);
            Destroy(Explosion, 2f);
            Destroy(this.gameObject);
            gameController.spaceships.Remove(this.gameObject);
            if (this.GetOwner() != null) Lose();
        }
        else
        {
            this.spaceshipStatistics.healthPoints += healthPoints;

        }
    }

    public bool CheckDistance(Ownable target)
    {
        if (target != null)
        {
            float distance = Vector3.Distance(target.transform.position, this.transform.position);
            return distance < 10.0 ? true : false;
        }
        return false;
    }
    public bool CheckDistance(Star target)
    {
        if (target != null)
        {
            float distance = Vector3.Distance(target.transform.position, this.transform.position);
            return distance < 10.0 ? true : false;
        }
        return false;
    }


    private void TurnEnginesOff()
    {
        if (burster != null)
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

    public string getModel()
    {
        return this.model;
    }
}


