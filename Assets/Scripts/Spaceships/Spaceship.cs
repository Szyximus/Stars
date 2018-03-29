using Assets.Scripts;
using Assets.Scripts.HexLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Spaceship : MonoBehaviour
{
    public HexCoordinates coordinates { get; set; }

    public int Speed { get; set; }

    int i = 0; //for the movement test, remove later

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
        if (Input.GetButton("MouseRight")) //Silly test of movement
        {
            if (EventManager.selectionManager.SelectedObject != null)
            {
                if (EventManager.selectionManager.SelectedObject.tag == "Unit")
                {
                    (EventManager.selectionManager.SelectedObject.GetComponent("Spaceship") as Spaceship).Move((EDirection)(i));
                    i++;
                    if (i > 5) i = 0;
                    Thread.Sleep(100);  //ugly way of not running command couple times during one click
                }
            }
        }

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
