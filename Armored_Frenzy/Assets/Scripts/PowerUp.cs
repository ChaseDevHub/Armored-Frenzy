using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PowerName { Boost, Shield}
public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private PowerName Ability;

    [SerializeField]
    private int Health;

    private Player player;

    //Color oriented: Read the material. If one color, assign one ability, if another color assign the other ability

    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        if(Health == 0)
        {
            Health = 10;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(Health == 0)
        {
            this.gameObject.SetActive(false);
            if (player.Inventory[0] == null)
            {
                player.Inventory[0] = this.gameObject;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Bullet") && Health > 0)
        {
            Health -= other.gameObject.GetComponent<Bullet>().DamagePoint;
        }
    }
}
