using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerClass : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private int ID;

    public static int ButtonID;

    public void OnSelect(BaseEventData eventData)
    {
        if (eventData.selectedObject)
        {
            UIMenu.TrackID = ID;
            //ButtonID = ID; //testing to see if I can call this instead
        }
    }
}
