using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
public class SideMenu : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (EventManager.selectionManager.SelectedObject == null)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
           // this.gameObject.GetComponent("MoveButton").SetActive(true);
        }

    }
}