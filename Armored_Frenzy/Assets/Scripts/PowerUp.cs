using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerName { Boost, Shield}
public class PowerUp : MonoBehaviour
{
    public PowerName Ability;
    //Call audio

    Audio gameAudio;

    // Start is called before the first frame update
    void Start()
    {
        Ability = PowerName.Boost;

        if(gameAudio == null)
        {
            gameAudio = GameObject.Find("ItemPickUp").GetComponent<Audio>();
        }
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
            gameAudio.PlayStart();
        }
    }
    
}
