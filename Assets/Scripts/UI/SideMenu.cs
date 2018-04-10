using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
public class SideMenu : MonoBehaviour
{
    bool shown, animating = false;
    RectTransform rectTransform;
    Text label;
    Button button;

    // Use this for initialization
    void Start()
    {
        //RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        transform.position += new Vector3(170, 0, 0);
        label = GetComponentInChildren<Text>();
        button = GetComponentInChildren<Button>();


    }

    // Update is called once per frame
    void Update()
    {

        if (EventManager.selectionManager.SelectedObject == null && shown && !animating )
        {
            StartCoroutine(Hide());
        }

        if (EventManager.selectionManager.SelectedObject != null && !shown && !animating)
        {
            StartCoroutine(Show());
        }
        if(shown) label.text = EventManager.selectionManager.SelectedObject.name;
        if (EventManager.selectionManager.SelectedObject != null && EventManager.selectionManager.SelectedObject.tag == "Unit") button.gameObject.SetActive(true); else button.gameObject.SetActive(false);
    }

    IEnumerator Show() {


        animating = true;
        float startTime = Time.time;
        Vector3 direction = new Vector3(-170, 0, 0);
        var endPos = transform.position + direction;

        while (Time.time - startTime < 0.25) //the movement takes exactly 0,25 s. regardless of framerate
        {

            transform.position += direction * Time.deltaTime * 4;
            yield return null;
        }
        transform.position = endPos;
        shown = true;
        animating = false;
    }

    IEnumerator Hide()
    {

        animating = true;
        float startTime = Time.time;
        Vector3 direction = new Vector3(170, 0, 0);
        var endPos = transform.position + direction;

        while (Time.time - startTime < 0.25) //the movement takes exactly 0,25 s. regardless of framerate
        {

            transform.position += direction * Time.deltaTime * 4;
            yield return null;
        }
        transform.position = endPos;
        label.text = " ";
        shown = false;
        animating = false;
    }
}