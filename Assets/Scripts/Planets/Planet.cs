using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{ 
    public HexCoordinates coordinates { get; set; }
    private MyUIHoverListener uiListener;

    // Use this for initialization
    void Start()
    {
        UpdateCoordinates();
        uiListener = GameObject.Find("WiPCanvas").GetComponent<MyUIHoverListener>();
    }

    void UpdateCoordinates()
    {
        coordinates = HexCoordinates.FromPosition(gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (!uiListener.isUIOverride) EventManager.selectionManager.SelectedObject = this.gameObject;
    }
}
