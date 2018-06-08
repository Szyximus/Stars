using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotate : MonoBehaviour {
    public float SpeedMultiplier;

    //rotates the sky every frame for more dynamism
    void Awake()
    {
        Application.targetFrameRate = 60; //fpscap
    }

    // Update is called once per frame
    void Update()
    {
        //Sets the float value of "_Rotation", adjust it by Time.time and a multiplier.
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * SpeedMultiplier);
    }
}
