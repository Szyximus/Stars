using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityController : MonoBehaviour {
    public GameObject hexCells;
    public GameObject ship1;
    public GameObject ship2;
    public Material m1, m2, m3;
    public int[] mark;
    // Use this for initialization
    void Start () {
        mark = new int[hexCells.transform.childCount];
        for (int i = 0; i < mark.Length; i++) mark[i] = -1;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 positionShip1 = ship1.transform.position;
        Vector3 positionShip2 = ship2.transform.position;
        for (int i = 2; i < hexCells.transform.childCount; i++)
        {
            Vector3 actualPosition = hexCells.transform.GetChild(i).transform.TransformPoint(Vector3.zero);
            float distance1 = Vector3.Distance(positionShip1, actualPosition);
            float distance2 = Vector3.Distance(positionShip2, actualPosition);
            if (distance1 < 10 || distance2 < 10)
            {
                mark[i] = 1;
            }
            else if (mark[i] == 1)
            {
                mark[i] = 0;
            }
        }

        for (int i = 2; i < hexCells.transform.childCount; i++)
            if (mark[i] == -1)
            {
                Material[] m123 = new Material[1] { m1 };
                hexCells.transform.GetChild(i).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials = m123;
            }
            else if (mark[i] == 0)
            {
                Material[] m123 = new Material[1] { m2 };
                hexCells.transform.GetChild(i).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials = m123;

            }
            else
            {
                Material[] m123 = new Material[1] { m3 };
                hexCells.transform.GetChild(i).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials = m123;

            }
    }
}
