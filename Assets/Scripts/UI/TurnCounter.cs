using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class TurnCounter : MonoBehaviour
{

    Text label;
    Text _Text;

    private GameController gameController;

    void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        //RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        label = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameController == null)
        {
            return;
        }

        label.text = "Year: " + (gameController.GetYear() + 2400).ToString();
    }
}