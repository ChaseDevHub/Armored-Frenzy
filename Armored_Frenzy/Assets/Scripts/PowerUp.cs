using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerName { Boost, Shield}
public class PowerUp : MonoBehaviour
{
    public PowerName Ability;

    [SerializeField]
    private int Health;

    private int ConstHealth;

    private Player player;

    //Color oriented: Maybe assign the material/shader based on the ability type it randomly gets

    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        if(Health == 0)
        {
            Health = 4;
        }

        ConstHealth = Health;

        int ran = Random.Range(0, 2);

        switch (ran)
        {
            case 0:
                Ability = PowerName.Boost;
                break;
            case 1:
                Ability = PowerName.Shield;
                break;
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

    public void ResetHealth()
    {
        Health = ConstHealth;
    }
}
