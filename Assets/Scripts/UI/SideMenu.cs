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
    public Text OwnerName;
    Button button;
    Button colonizeButton;

    // Use this for initialization
    void Start()
    {
        //RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        transform.position += new Vector3(170, 0, 0);
        label = GameObject.Find("Name").GetComponent<Text>();
        colonizeButton = GameObject.Find("ColonizeButton").GetComponent<Button>();
        OwnerName = GameObject.Find("OwnerName").GetComponent<Text>();


    }

    // Update is called once per frame
    void Update()
    {

        if (EventManager.selectionManager.SelectedObject == null && shown && !animating)
        {
            StartCoroutine(Hide());
        }

        if (EventManager.selectionManager.SelectedObject != null && !shown && !animating)
        {
            StartCoroutine(Show());
        }
        if (shown && EventManager.selectionManager.SelectedObject != null) label.text = EventManager.selectionManager.SelectedObject.name;

        if (EventManager.selectionManager.SelectedObject != null && (EventManager.selectionManager.SelectedObject.GetComponent<Ownable>() as Ownable) != null)
        {
            OwnerName.text = "Owner: " + (EventManager.selectionManager.SelectedObject.GetComponent<Ownable>() as Ownable).GetOwnerName();
            OwnerName.gameObject.SetActive(true);
        }
        else
        {
            OwnerName.gameObject.SetActive(false);
        }

        if (EventManager.selectionManager.SelectedObject != null && EventManager.selectionManager.SelectedObject.tag == "Unit")
        {
            colonizeButton.gameObject.SetActive(true);
        }
        else
        {
            colonizeButton.gameObject.SetActive(false);
        }

    }

    IEnumerator Show()
    {


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