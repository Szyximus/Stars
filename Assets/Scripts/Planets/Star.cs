using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public HexCoordinates coordinates { get; set; }

    // Use this for initialization
    void Start()
    {
        UpdateCoordinates();
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
        EventManager.selectionManager.SelectedObject = this.gameObject;
    }
}
