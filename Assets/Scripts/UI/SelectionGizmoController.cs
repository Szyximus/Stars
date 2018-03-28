using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Assets.Scripts;


public class SelectionGizmoController : MonoBehaviour
{
    SelectionGizmoController main;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(-100, -100, -100);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("MouseLeft")){
            if (EventManager.selectionManager.SelectedObject != null) { transform.position = EventManager.selectionManager.SelectedObject.transform.position; }
            else transform.position = new Vector3(-100, -100, -100);
        }

        transform.Rotate(Vector3.up * 10 * Time.deltaTime);

    }
    void Awake()
    {
        main = this;

    }
}
