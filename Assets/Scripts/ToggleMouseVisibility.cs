using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Hide and show the mouse when it is overa UI element
public class ToggleMouseVisibility : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{

    // https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerEnterHandler.html
    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.visible = true;
    }

    //https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerExitHandler.html
    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.visible = false;
    }
}
