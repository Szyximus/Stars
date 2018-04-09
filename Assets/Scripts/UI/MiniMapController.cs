using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections;

public class MiniMapController : MonoBehaviour
    , IPointerDownHandler
{

    //Singleton
    public static MiniMapController main;
    CameraController controller; //Main camera Controller


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
        MainCamera = (GameObject.Find("CameraRig").GetComponentInChildren<Camera>());
        MiniMapCamera = (GameObject.Find("MiniMapCamera").GetComponent<Camera>());
        //LineMat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/WhiteMinimap.mat", typeof(Material));

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
        


    }

    void Update()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        if (Input.GetButton("MouseLeft") && Input.mousePosition.x > 8 && Input.mousePosition.x < rectTransform.rect.width + 8 && Input.mousePosition.y > 8 && Input.mousePosition.y < rectTransform.rect.height + 8 )
        {
            controller.LookAt(lastClicked);
        }
        //Bottom left corner
        Ray ray1 = MainCamera.ScreenPointToRay(new Vector3(1, 1, 0));

        //Bottom right
        Ray ray2 = MainCamera.ScreenPointToRay(new Vector3(Screen.width - 1, 1, 0));

        //Top right
        Ray ray3 = MainCamera.ScreenPointToRay(new Vector3(Screen.width -1, Screen.height - 62, 0));

        //Top Left
        Ray ray4 = MainCamera.ScreenPointToRay(new Vector3(1, Screen.height - 62, 0));

        //Find world co-ordinates
        RaycastHit hit;
        Physics.Raycast(ray1, out hit, 500, (1 << 9));
        Vector3 v1 = hit.point;

        Physics.Raycast(ray2, out hit, 500, (1 << 9));
        Vector3 v2 = hit.point;

        Physics.Raycast(ray3, out hit, 500, (1 << 9));
        Vector3 v3 = hit.point;

        Physics.Raycast(ray4, out hit, 500, (1 << 9));
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
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        Vector2 Target;
        Target.x = eventData.position.x - 8;
        Target.y = eventData.position.y - 8;

        // calculate camera position from minimap click:
        Target.x = MiniMapCamera.transform.position.x + (Target.x - rectTransform.rect.width / 2f)/ (rectTransform.rect.height / 2) * MiniMapCamera.orthographicSize;
        Target.y = MiniMapCamera.transform.position.z + (Target.y - rectTransform.rect.height / 2f)/(rectTransform.rect.height/2) * MiniMapCamera.orthographicSize;
        lastClicked = Target;


    }
}
