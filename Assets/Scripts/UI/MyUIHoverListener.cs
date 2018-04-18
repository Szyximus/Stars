using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MyUIHoverListener : MonoBehaviour
{
    public bool IsUIOverride { get; private set; }

    void Update()
    {
        // It will turn true if hovering any UI Elements
        IsUIOverride = EventSystem.current.IsPointerOverGameObject();
    }
}