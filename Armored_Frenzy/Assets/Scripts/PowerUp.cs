using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerName { Boost, Shield}
public class PowerUp : MonoBehaviour
{
    public PowerName Ability;

    private bool Collected;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        Collected = false;

        int ran = 0;//Random.Range(0, 2);

        switch (ran)
        {
            case 0:
                Ability = PowerName.Boost;
                break;
            /*case 1:
                Ability = PowerName.Shield; 
                break;*/
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(Collected) //if powerup has been collected, turn it off/deactivate
        {
            this.gameObject.SetActive(false);
            if (player.Inventory[0] == null)
            {
                player.Inventory[0] = this.gameObject;
            }
        }
    }

    /* 
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Bullet")) 
        {
            Collected = true;
        }
    }*/

    public void ResetPowerUp()
    {
        Collected = false;
    }
}
