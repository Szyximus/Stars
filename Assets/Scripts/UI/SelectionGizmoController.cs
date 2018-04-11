using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Assets.Scripts;


public class SelectionGizmoController : MonoBehaviour
{
    SelectionGizmoController main;
    Vector3 offset = new Vector3(0, 0.1f, 0);

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(-1000, -1000, -1000);

    }

    // Update is called once per frame
    void Update()
    {
        //This should only be called at an event, not every frame. fix later after EventManager is proper.
        if (EventManager.selectionManager.SelectedObject != null)
        {
            transform.position = EventManager.selectionManager.SelectedObject.transform.position + offset;
        }
        else transform.position = new Vector3(-1000, -1000, -1000);

        transform.Rotate(Vector3.up * 20 * Time.deltaTime);

    }
    void Awake()
    {
        main = this;

    }
}
