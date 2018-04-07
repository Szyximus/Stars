﻿using Assets.Scripts;
using Assets.Scripts.HexLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Spaceship : MonoBehaviour
{
    public HexCoordinates Coordinates { get; set; }

    public int Speed { get; set; }

    public HexCoordinates Destination { get; set; }

    int i = 0; //for the movement test, remove later

    // Use this for initialization
    void Start()
    {
        //DEBUG
        Speed = 1;

        UpdateCoordinates();
    }

    void UpdateCoordinates()
    {
        Coordinates = HexCoordinates.FromPosition(gameObject.transform.position);
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
                transform.Translate(r, 0, r_sqrt3, Space.Self);
                break;
            case EDirection.Right:
                transform.Translate(2*r, 0, 0, Space.Self);
                break;
            case EDirection.BottomRight:
                transform.Translate(r, 0, -r_sqrt3, Space.Self);
                break;
            case EDirection.BottomLeft:
                transform.Translate(-r, 0, -r_sqrt3, Space.Self);
                break;
            case EDirection.Left:
                transform.Translate(-2*r, 0, 0, Space.Self);
                break;
            case EDirection.TopLeft:
                transform.Translate(-r, 0, r_sqrt3, Space.Self);
                break;
        }

        UpdateCoordinates();
    }

    /*
     * TODO: Probably this function will be called from some round update 
     */
    public void MoveTo( HexCoordinates dest )
    {
        //while (Coordinates != dest)
            for (int i = Speed; i > 0; i--)
            {
                if (dest.Z > Coordinates.Z && dest.X >= Coordinates.X)
                    Move(EDirection.TopRight);
                else if (dest.Z > Coordinates.Z && dest.X < Coordinates.X)
                    Move(EDirection.TopLeft);
                else if (dest.Z < Coordinates.Z && dest.X > Coordinates.X)
                    Move(EDirection.BottomRight);
                else if (dest.Z < Coordinates.Z && dest.X <= Coordinates.X)
                    Move(EDirection.BottomLeft);
                else if (dest.X > Coordinates.X)
                    Move(EDirection.Right);
                else if (dest.X < Coordinates.X)
                    Move(EDirection.Left);
            }
    }

    public void DoTestStuff()
    {
        //if (Input.GetButton("MouseRight")) //Silly test of movement
        {
            if (EventManager.selectionManager.SelectedObject != null)
            {
                if (EventManager.selectionManager.SelectedObject.tag == "Unit")
                {
                    (EventManager.selectionManager.SelectedObject.GetComponent("Spaceship") as Spaceship).Move((EDirection)i);
                    i++;
                    if (i > 5) i = 0;

                    Debug.Log(string.Format("Destination: {0}", Destination) );
                }
            }
        }
    }
}