
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float panSpeed = 10f;
    public float panBorderThickness = 10f;

    public Vector3 cameraMax;
    public Vector3 cameraMin;

    public float scrollSpeed = 10f;


    // Update is called once per frame
    void Update () {

        Vector3 pos = transform.position;
        Vector3 rot = transform.localEulerAngles;

        if (Input.GetKey("w") || Input.GetKey("up") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * 2 * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.GetKey("down") || Input.mousePosition.y <=  panBorderThickness)
        {
            pos.z -= panSpeed * 2 * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.GetKey("left") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * 2 * Time.deltaTime;
        }

        if (Input.GetKey("d") || Input.GetKey("right") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * 2 * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed;
        pos.z += scroll * scrollSpeed * 0.57735f; //tan(60deg)

        pos.x = Mathf.Clamp(pos.x, cameraMin.x, cameraMax.x);
        pos.y = Mathf.Clamp(pos.y, cameraMin.y, cameraMax.y);
        pos.z = Mathf.Clamp(pos.z, cameraMin.z, cameraMax.z);

        rot.x = 60 - ((cameraMax.y - pos.y) / cameraMax.y) * 40;



        transform.position = pos;
        transform.localEulerAngles = rot;

    }
}
