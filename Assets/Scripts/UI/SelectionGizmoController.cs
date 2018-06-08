using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Assets.Scripts;


public class SelectionGizmoController : MonoBehaviour
{
    SelectionGizmoController main;
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
        if (EventManager.selectionManager.SelectedObject != null)
        {
            if (transform.position != EventManager.selectionManager.SelectedObject.transform.position + offset)
            {
                transform.position = EventManager.selectionManager.SelectedObject.transform.position + offset;
                sound.Play();
            }

        }
        else
        {
            if (transform.position != new Vector3(-1000, -1000, -1000))
            {
                transform.position = new Vector3(-1000, -1000, -1000);
                sound.Play();
            }
            EventManager.selectionManager.TargetObject = null;
            
        }

        //if (transform.hasChanged)
        //{
        //    EventManager.selectionManager.TargetObject = null;
        //    sound.Play();
        //    transform.hasChanged = false;
        //}

        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
        //transform.hasChanged = false;

    }
    
}
