using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickableImage : MonoBehaviour 
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerUpHandler
{
    private AudioSource sound;
    private Shadow shadow;
    // Use this for initialization
    void Start () {
        sound = gameObject.GetComponent<AudioSource>();
        shadow = gameObject.GetComponent<Shadow>();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Transform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
        shadow.effectDistance = new Vector2(0,0);
   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        shadow.effectDistance = new Vector2(5, -4);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        sound.Play();
    }
}
