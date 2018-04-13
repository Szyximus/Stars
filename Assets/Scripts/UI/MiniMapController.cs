using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class MiniMapController : MonoBehaviour
    , IPointerDownHandler
{

    //Singleton
    public static MiniMapController main;
    CameraController controller; //Main camera Controller

    HexGrid grid;


    Camera MainCamera;
    Camera MiniMapCamera;
    public Material LineMat; //Material of the camera trapezoid

    Vector2 lastClicked;

    private LineRenderer line;

    Vector3 a;
    Vector3 b;
    Vector3 c;
    Vector3 d;

    RectTransform rectTransform; //Transform data of the minimap, it scales with the resolution




    void Awake()
    {
        main = this;

    }

    void Start()
    {
        grid = (GameObject.Find("HexGrid").GetComponent("HexGrid") as HexGrid);
        MainCamera = (GameObject.Find("CameraRig").GetComponentInChildren<Camera>());
        MiniMapCamera = (GameObject.Find("MiniMapCamera").GetComponent<Camera>());

        controller = (GameObject.Find("CameraRig").GetComponent("CameraController") as CameraController);

        // Add a Line Renderer to the GameObject
        line = this.gameObject.AddComponent<LineRenderer>();
        // Set the width of the Line Renderer
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        // Set the number of vertex fo the Line Renderer
        line.positionCount = 5;
        line.material = LineMat;
        line.gameObject.layer = 8; // Set trapezoid layer to 8=minimap, so it doesn't show to the main camera

        //TODO: this finds home planet by name, needs better method
        //controller.transform.position = (new Vector3((GameObject.Find("HomePlanet").transform.position.x), 0, (GameObject.Find("HomePlanet").transform.position.z)));

    }

    void Update()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        if (Input.GetButton("MouseLeft") && Input.mousePosition.x > 8 && Input.mousePosition.x < rectTransform.rect.width + 8 && Input.mousePosition.y > 8 && Input.mousePosition.y < rectTransform.rect.height + 8)
        {
            controller.LookAt(lastClicked);
        }
        //Bottom left corner
        Ray ray1 = MainCamera.ScreenPointToRay(new Vector3(1, 1, 0));

        //Bottom right
        Ray ray2 = MainCamera.ScreenPointToRay(new Vector3(Screen.width - 1, 1, 0));

        //Top right
        Ray ray3 = MainCamera.ScreenPointToRay(new Vector3(Screen.width - 1, Screen.height - 62, 0));

        //Top Left
        Ray ray4 = MainCamera.ScreenPointToRay(new Vector3(1, Screen.height - 62, 0));

        //Find world co-ordinates
        RaycastHit hit;
        Physics.Raycast(ray1, out hit, 5000, (1 << 9));
        Vector3 v1 = hit.point;

        Physics.Raycast(ray2, out hit, 5000, (1 << 9));
        Vector3 v2 = hit.point;

        Physics.Raycast(ray3, out hit, 5000, (1 << 9));
        Vector3 v3 = hit.point;

        Physics.Raycast(ray4, out hit, 5000, (1 << 9));
        Vector3 v4 = hit.point;

        a.Set(50, 0, 60);
        b.Set(80, 0, 60);
        c.Set(80, 0, 30);
        d.Set(50, 0, 30);

        line.SetPosition(0, v1);
        line.SetPosition(1, v2);
        line.SetPosition(2, v3);
        line.SetPosition(3, v4);
        line.SetPosition(4, v1); //draw trapezoid

        UpdateCamera();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        Vector2 Target;
        Target.x = eventData.position.x - 8;
        Target.y = eventData.position.y - 8;

        // calculate camera position from minimap click:
        Target.x = MiniMapCamera.transform.position.x + (Target.x - rectTransform.rect.width / 2f) / (rectTransform.rect.height / 2) * MiniMapCamera.orthographicSize;
        Target.y = MiniMapCamera.transform.position.z + (Target.y - rectTransform.rect.height / 2f) / (rectTransform.rect.height / 2) * MiniMapCamera.orthographicSize;
        lastClicked = Target;
    }

    public void UpdateCamera() // Updates minimap based on discovered tiles
    {
        var cells = grid.cells.Where(c => (c.State == Assets.Scripts.HexLogic.EHexState.Visible || c.State == Assets.Scripts.HexLogic.EHexState.Hidden));
        if (cells.Any())
        {
            float minx = 1000;
            float minz = 1000;
            float maxx = -1000;
            float maxz = -1000;

            foreach (HexCell cell in cells)
            {
                float x = cell.transform.localPosition.x;
                float z = cell.transform.localPosition.z;
                if (x > maxx)
                {
                    maxx = x;
                }

                if (x < minx)
                {
                    minx = x;
                }

                if (z > maxz)
                {
                    maxz = z;
                }

                if (z < minz)
                {
                    minz = z;
                }
            }

            Vector3 corner1 = new Vector3(minx, MiniMapCamera.transform.position.y, minz);
            Vector3 corner2 = new Vector3(minx, MiniMapCamera.transform.position.y, maxz);
            Vector3 corner3 = new Vector3(maxx, MiniMapCamera.transform.position.y, minz);
            Vector3 corner4 = new Vector3(maxx, MiniMapCamera.transform.position.y, maxz);

            controller.boundMax = new Vector3(maxx, 0, maxz);
            controller.boundMin = new Vector3(minx, 0, minz);

            MiniMapCamera.transform.position = Vector3.Lerp(Vector3.Lerp(corner1, corner2, 0.5f), Vector3.Lerp(corner3, corner4, 0.5f), 0.5f);
            MiniMapCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(
                Vector3.Distance(
                    Vector3.Lerp(
                        corner1, corner3, 0.5f),
                    Vector3.Lerp(corner2, corner4, 0.5f)),
                Vector3.Distance(
                    Vector3.Lerp(
                        corner1, corner2, 0.5f),
                    Vector3.Lerp(corner3, corner4, 0.5f)))
                    / 2f + 2f;

        }

    }
}
