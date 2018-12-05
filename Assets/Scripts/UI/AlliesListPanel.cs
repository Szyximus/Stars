using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlliesListPanel : MonoBehaviour
{
    GameController gameController;
    Text alliesList;
    Text alliesList2;
    bool shown;
    // Use this for initialization
    void Start()
    {
        shown = false;
        gameObject.SetActive(false);
       
        alliesList = GetComponentsInChildren<Text>()[1];
        alliesList2 = GetComponentsInChildren<Text>()[0];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowAllies()
    {
        if (shown == false)
        {
            shown = true;
            gameObject.SetActive(true);
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
            ArrayList list = gameController.GetCurrentPlayer().GetAllies();
            foreach (Player player in list)
            {
                alliesList.text = player.name + "   ";

                Debug.Log(player.name);
                Debug.Log(alliesList.text);
            }
        }
        else
        {
            gameObject.SetActive(false);
            alliesList.text = "";
            shown = false;
        }
    }
}

