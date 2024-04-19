using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerClass : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private int ID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (eventData.selectedObject)
        {
            UIMenu.TrackID = ID;
        }
    }
}
