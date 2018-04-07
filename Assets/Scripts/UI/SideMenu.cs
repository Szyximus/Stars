using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Move ()
    {
        if (Input.GetButton("MouseRight")) //Silly test of movement
        {
            if (EventManager.selectionManager.SelectedObject.tag == "null")
            {
                this.gameObject.SetActive(false);
                this.gameObject.GetComponentInChildren("Move").SetActive(false);
            }
            else if (EventManager.selectionManager.SelectedObject.tag == "Unit")
            {
                this.gameObject.SetActive(true);
                this.gameObject.GetComponentInChildren("MoveButton").SetActive(true);
            }
            else if (EventManager.selectionManager.SelectedObject.tag == "Planet")
            {
                this.gameObject.SetActive(true);
                this.gameObject.GetComponentInChildren("MoveButton").SetActive(false);
                this.gameObject.GetComponentInChildren("TextMesh") = SelectedObject.tag;
            }
        }
    }
}
