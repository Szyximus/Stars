using Assets.Scripts;
using Assets.Scripts.HexLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public HexCoordinates coordinates { get; set; }

    public int Speed { get; set; } 

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

    public void Move( EDirection direction )
    {
        var r = HexMetrics.innerRadius;
        var r_sqrt3 = r * 1.7320508757f;


        switch (direction)
        {
            case EDirection.TopRight:
                transform.Translate(-r, 0, -r_sqrt3, Space.Self);
                break;
            case EDirection.Right:
                transform.Translate(-2*r, 0, 0, Space.Self);
                break;
            case EDirection.BottomRight:
                transform.Translate(-r, 0, r_sqrt3, Space.Self);
                break;
            case EDirection.BottomLeft:
                transform.Translate(r, 0, -r_sqrt3, Space.Self);
                break;
            case EDirection.Left:
                transform.Translate(2*r, 0, 0, Space.Self);
                break;
            case EDirection.TopLeft:
                transform.Translate(r, 0, -r_sqrt3, Space.Self);
                break;
        }
    }
}
