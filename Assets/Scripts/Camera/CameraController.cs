
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float panSpeed = 10f;
    public float panBorderThickness = 10f;
    public float orbitSpeed = 10f;
    public float smoothTime = 4f;

    public Vector3 cameraMax;
    public Vector3 cameraMin;

    public float scrollSpeed = 10f;

    Vector3 vel = Vector3.zero;


    // Update is called once per frame
    void Update() {

        Vector3 pos = transform.position;
        Vector3 rot = transform.localEulerAngles;
        var distance = pos.y / Mathf.Sin(rot.x * Mathf.Deg2Rad);

        if (Input.GetKey("w") || Input.GetKey("up") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            vel.z += 0.1f * distance * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.GetKey("down") || Input.mousePosition.y <= panBorderThickness)
        {
            vel.z -= 0.1f * distance * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.GetKey("left") || Input.mousePosition.x <= panBorderThickness)
        {
            vel.x -= 0.1f * distance * Time.deltaTime;
        }

        if (Input.GetKey("d") || Input.GetKey("right") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            vel.x += 0.1f * distance * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        vel.y -= scroll * scrollSpeed * distance * 0.002f;
 
        if ((scroll< 0 && pos.y >= cameraMin.y)  && (scroll > 0 && pos.y <= cameraMax.y))
        {
        vel.z += scroll* scrollSpeed * distance * 0.001f; //tan(60deg) = 0.57735f
        }
     

        if (Input.GetMouseButton(2))
        {
            vel.x -= panSpeed * Input.GetAxis("Mouse X") * distance * 0.0004f;
            vel.z -= panSpeed * Input.GetAxis("Mouse Y") * distance * 0.0003f;
        }

        //rotationYAxis += velocityY;
        //Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        //Quaternion toRotation = Quaternion.Euler(0, rotationYAxis, 0);
        //Quaternion rotation = toRotation;
        vel.x = Mathf.Clamp(vel.x, -panSpeed, panSpeed);
        vel.y = Mathf.Clamp(vel.y, -panSpeed, panSpeed);
        vel.z = Mathf.Clamp(vel.z, -panSpeed, panSpeed);

        vel.x = Mathf.Lerp(vel.x, 0, Time.deltaTime * smoothTime);
        vel.y = Mathf.Lerp(vel.y, 0, Time.deltaTime * smoothTime);
        vel.z = Mathf.Lerp(vel.z, 0, Time.deltaTime * smoothTime);



        pos += vel;

        pos.x = Mathf.Clamp(pos.x, cameraMin.x, cameraMax.x);
        pos.y = Mathf.Clamp(pos.y, cameraMin.y, cameraMax.y);
        pos.z = Mathf.Clamp(pos.z, cameraMin.z, cameraMax.z);

        rot.x = 60 - ((cameraMax.y - pos.y) / cameraMax.y) * 40;



        transform.position = pos;
        transform.localEulerAngles = rot;

    }
}
