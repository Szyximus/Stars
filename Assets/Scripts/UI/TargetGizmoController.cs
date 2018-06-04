using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Assets.Scripts;


public class TargetGizmoController : MonoBehaviour
{
    TargetGizmoController main;
    Vector3 offset = new Vector3(0, 0.01f, 0);
    private AudioSource sound;

    void Awake()
    {
        main = this;
    }

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(-1000, -1000, -1000);
        sound = gameObject.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        //This should only be called at an event, not every frame. fix later after EventManager is proper.
        if (EventManager.selectionManager.TargetObject != null)
        {
            transform.position = EventManager.selectionManager.TargetObject.transform.position + offset;
        }
        else transform.position = new Vector3(-1000, -1000, -1000);

        if (transform.hasChanged)
        {
            sound.Play();
            transform.hasChanged = false;
        }
        transform.Rotate(Vector3.up * -10 * Time.deltaTime);
        transform.hasChanged = false;

    }
    
}
