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
        else if (EventManager.selectionManager.SelectedObject.tag == "Unit")
        {
            this.gameObject.SetActive(true);
           // this.gameObject.GetComponent("MoveButton").SetActive(true);
        }
        else if (EventManager.selectionManager.SelectedObject.tag == "Planet")
        {
            this.gameObject.SetActive(true);
            //this.gameObject.GetComponent("MoveButton").SetActive(false);
            //this.gameObject.GetComponent("TextMesh") = SelectedObject.tag;
        }

    }
}