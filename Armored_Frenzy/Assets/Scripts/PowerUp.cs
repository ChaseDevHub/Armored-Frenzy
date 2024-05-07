using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerName { Boost, Shield}
public class PowerUp : MonoBehaviour
{
    public PowerName Ability;
    //Call audio
    
    // Start is called before the first frame update
    void Start()
    {
        Ability = PowerName.Boost;
    }


    // Update is called once per frame
    void Update()
    {
        
   
    }

    //Adding audio sound effect here
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
        }
    }
    
}
