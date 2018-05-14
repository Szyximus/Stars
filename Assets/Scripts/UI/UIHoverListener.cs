using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIHoverListener : MonoBehaviour
{
    public bool IsUIOverride { get; private set; }

    void Update()
    {
        // It will turn true if hovering any UI Elements, used to avoid clicking at objects through UI
        IsUIOverride = EventSystem.current.IsPointerOverGameObject();
    }
}