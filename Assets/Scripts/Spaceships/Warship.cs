using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Linq;

/**
 * Class represent War spaceship.
 */
public class Warship : Spaceship
{

    private void Awake()
    {
        MaxActionPoints = 5;
        RadarRange = 25;
        buildCost = 13;
    }


}