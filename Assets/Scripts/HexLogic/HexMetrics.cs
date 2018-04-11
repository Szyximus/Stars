using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMetrics : MonoBehaviour
{

    public const float outerRadius = 5f; // Hexagon radius on the long symmetry
    public const float innerRadius = outerRadius * 0.866025404f; // Hexagon radius on the short symmetry
    public const int chunkSizeX = 10, chunkSizeZ = 10;

    public static Vector3[] corners = { //Hexagon corners
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };
}
