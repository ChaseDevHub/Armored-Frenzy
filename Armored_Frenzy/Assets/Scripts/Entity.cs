using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    /*
     Fields to have:

    Speed - float 
    Direction - Vector3
    Boost - float (an addition to the speed: Speed = Speed + Booast) for a certain time: 3 seconds
    Inventory -> Array inventory = new Array[1];
     */

    public float Speed;
    public float Boost;
    public Vector3 Direction;
    public Vector3 Rotation;
    public Array Inventory = new Array[1];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
