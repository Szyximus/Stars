using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class VisibilityController : MonoBehaviour {
    private HexGrid grid;

    void Start () {
        grid = (GameObject.Find("HexGrid").GetComponent("HexGrid") as HexGrid);
    }

	
	// Update is called once per frame
	void Update () {
       
    }

    public void SetupNewTurn(Player oldPlayer, Player currentPlayer)
    {
        
    }
}
