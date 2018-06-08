using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ErrorInfoPanel : MonoBehaviour
{

    private static ErrorInfoPanel instance;
    bool Shown;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("ErrorInfoPanel Awake");
        } else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        transform.Find("ErrorInfo").gameObject.SetActive(false);
        Shown = false;
    }

    public void Show(string info)
    {
        transform.Find("ErrorInfo/Description").GetComponent<Text>().text = info;

        transform.Find("ErrorInfo").gameObject.SetActive(true);
        Shown = true;
    }

    void Hide()
    {
        transform.Find("ErrorInfo").gameObject.SetActive(false);
        Shown = false;
    }

    public void Toggle()
    {
        if (Shown) Hide();
        else Show("");
    }
}
