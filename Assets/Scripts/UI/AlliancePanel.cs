using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;

public class AlliancePanel : MonoBehaviour
{
    public bool makeAlliance = false;

    Button yesButton;
    Button noButton;
    GameObject allianceText;
    Text label;
    GameObject alliancePanel;
    public bool buttonClicked = false;
   
    private GameController gameController;

    
    public void Init()
    {
        gameObject.SetActive(false);

        allianceText = GameObject.Find("AllianceText");
       // label = allianceText.GetComponent<Text>();
        Debug.Log(allianceText + "fdsfsd");
        //  yesButton = GameObject.Find("YesButton").GetComponent<Button>();
        //  noButton = GameObject.Find("NoButton").GetComponent<Button>();

    }
    // Use this for initialization
    void Start()
    {
       
    }

    public void ActionAfterClickYes()
    {
        makeAlliance = true;
        gameObject.SetActive(false);
        Debug.Log("yes");
        Debug.Log(allianceText + "fdsfsd");
        buttonClicked = true;
    }

    public void ActionAfterClickNo()
    {
        makeAlliance = false;
        gameObject.SetActive(false);
        Debug.Log("no");
        buttonClicked = true;
    }
    public void SetPlayerNameToAlliance(string playerName)
    {
        gameObject.SetActive(true);
      //  allianceText.text = playerName + " wants to make an alliance. Are you agree ? ";
        Debug.Log("sdasdas");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
