using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PowerName { Boost, Shield}
public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private PowerName Ability;

    //Color oriented: Read the material. If one color, assign one ability, if another color assign the other ability

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
