using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MiniMapController : MonoBehaviour
    , IPointerDownHandler
{

    //Singleton
    public static MiniMapController main;
    public CameraController controller;


    public Camera MainCamera;
    public Camera MiniMapCamera;

    public Material mat;
    Vector2 lastClicked;

    void Awake()
    {
        main = this;

    }

    void Update()
    {
        if (Input.GetButton("MouseLeft") && Input.mousePosition.x > 8 && Input.mousePosition.x < 256 + 8 && Input.mousePosition.y > 8 && Input.mousePosition.y < 192 + 8 )
        {
            controller.LookAt(lastClicked);
        }  
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        Vector2 Target;
        Target.x = eventData.position.x - 8;
        Target.y = eventData.position.y - 8;

        Target.x = MiniMapCamera.transform.position.x + (Target.x - 256 / 2f)/96 * MiniMapCamera.orthographicSize;
        Target.y = MiniMapCamera.transform.position.z + (Target.y - 192 / 2f)/96 * MiniMapCamera.orthographicSize;
        lastClicked = Target;
        //controller.LookAt(Target);


        Debug.Log("touched at " + Target);
    }
}
