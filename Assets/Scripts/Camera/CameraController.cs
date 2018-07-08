/*
    Copyright (c) 2018, Szymon Jakóbczyk, Paweł Płatek, Michał Mielus, Maciej Rajs, Minh Nhật Trịnh, Izabela Musztyfaga
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation 
          and/or other materials provided with the distribution.
        * Neither the name of the [organization] nor the names of its contributors may be used to endorse or promote products derived from this software 
          without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
    HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
    USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using UnityEngine;

public class CameraController : MonoBehaviour {

    /// <summary>
    /// Controls the camera, uses keyboard and mouse input
    /// </summary>

    public static CameraController main;

    public float panSpeed = 10f;
    public float panBorderThickness = 10f;
    public float orbitSpeed = 10f;
    public float smoothTime = 4f;

    public Vector3 boundMax;
    public Vector3 boundMin;

    public float zoomSpeed = 10f;

    Vector3 vel = Vector3.zero;
    float rotVel = 0f;
    float zoomVel = 0f;

    void Awake()
    {
        main = this;
    }

    // Update is called once per frame
    void Update() {

        Vector3 pos = transform.position;
        Vector3 rot = transform.localEulerAngles;
        Vector3 scale = transform.localScale;
        var zoom = scale.z;

        if (Input.GetAxisRaw("Vertical") > 0 || ((Input.mousePosition.y >= Screen.height - panBorderThickness) && Input.GetButton("MouseMiddle") && (Input.GetButton("Control") == false)))
        {
            vel.z += zoom * Time.deltaTime * 5;
        }

        if (Input.GetAxisRaw("Vertical") < 0 || ((Input.mousePosition.y <= panBorderThickness) && Input.GetButton("MouseMiddle") && (Input.GetButton("Control") == false)))
        {
            vel.z -= zoom * Time.deltaTime * 5;
        }

        if (Input.GetAxisRaw("Horizontal") < 0 || ((Input.mousePosition.x <= panBorderThickness) && Input.GetButton("MouseMiddle") && (Input.GetButton("Control") == false)))
        {
            vel.x -= zoom * Time.deltaTime * 5;
        }

        if (Input.GetAxisRaw("Horizontal") > 0 || ((Input.mousePosition.x >= Screen.width - panBorderThickness) && Input.GetButton("MouseMiddle") && (Input.GetButton("Control") == false)))
        {
            vel.x += zoom * Time.deltaTime * 5;
        }

        if (Input.GetButton("Plus"))
        {
            zoomVel = -0.01f;
        }

        if (Input.GetButton("Minus"))
        {
            zoomVel = 0.01f;
        }


        float scroll = Input.GetAxis("Mouse ScrollWheel");

        zoomVel -= scroll * zoomSpeed * zoom * 0.005f;
     
    
        if (Input.GetButton("MouseMiddle")
           && (Input.GetButton("Control") == false)
           && (Input.mousePosition.x >= panBorderThickness && Input.mousePosition.x <= Screen.width - panBorderThickness && Input.mousePosition.y >= panBorderThickness && Input.mousePosition.y <= Screen.height - panBorderThickness))
        {
            vel.x -= panSpeed * Input.GetAxis("Mouse X") * zoom * 0.02f;
            vel.z -= panSpeed * Input.GetAxis("Mouse Y") * zoom * 0.02f;
        }

        if (Input.GetButton("MouseMiddle") && Input.GetButton("Control") && Input.mousePosition.x >= panBorderThickness && Input.mousePosition.x <= Screen.width - panBorderThickness && Input.mousePosition.y >= panBorderThickness && Input.mousePosition.y <= Screen.height - panBorderThickness)
        {
            rotVel -= panSpeed * Input.GetAxis("Mouse X") * 0.05f;
        }

        vel.x = Mathf.Clamp(vel.x, -panSpeed, panSpeed);
        vel.y = Mathf.Clamp(vel.y, -panSpeed, panSpeed);
        vel.z = Mathf.Clamp(vel.z, -panSpeed, panSpeed);

        vel.x = Mathf.Lerp(vel.x, 0, Time.deltaTime * smoothTime);
        vel.y = Mathf.Lerp(vel.y, 0, Time.deltaTime * smoothTime);
        vel.z = Mathf.Lerp(vel.z, 0, Time.deltaTime * smoothTime);

        pos += vel;

        zoomVel = Mathf.Lerp(zoomVel, 0, Time.deltaTime * smoothTime);

        scale.x += zoomVel;
        scale.y += zoomVel;
        scale.z += zoomVel;

        rotVel = Mathf.Clamp(rotVel, -180, 180);

        rotVel = Mathf.Lerp(rotVel, 0, Time.deltaTime * smoothTime);

        rot.x = zoom  * 40 + 20f;
        rot.y += rotVel;

        if (Input.GetButtonUp("Control"))
        {
            rot.y = 0;
            rotVel = 0;
        }


        scale.x = Mathf.Clamp(scale.x, 0.2f, 1f);
        scale.y = Mathf.Clamp(scale.y, 0.2f, 1f);
        scale.z = Mathf.Clamp(scale.z, 0.2f, 1f);

        pos.x = Mathf.Clamp(pos.x, boundMin.x, boundMax.x);
        pos.y = Mathf.Clamp(pos.y, boundMin.y, boundMax.y);
        pos.z = Mathf.Clamp(pos.z, boundMin.z, boundMax.z);

        transform.position = pos;
        transform.localScale = scale;
        transform.localEulerAngles = rot;

    }

    public void LookAt (Vector2 Target)
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, Target.x, 0.5f);
        pos.z = Mathf.Lerp(pos.z, Target.y, 0.5f);
        transform.position =pos;
    }
}
