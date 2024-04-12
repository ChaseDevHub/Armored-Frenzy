using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningDestroyable : MonoBehaviour
{
    [SerializeField]
    private Vector3 Rotation;

    [SerializeField]
    private float SpeedRotation;

    [SerializeField]
    bool RandomizeXAxis;

    // Start is called before the first frame update
    void Start()
    {
        int tempX = 20;
        if (RandomizeXAxis)
        {
            tempX = Random.Range(20, 150);
        }
        
        int tempY = Random.Range(20, 150);
        int tempZ = Random.Range(20, 150);

        //float tempSpeed = Random.Range(0.15f, 0.25f);

        //Rotation = new Vector3(0, 0, 115);

        SpeedRotation = .25f;

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Rotation * SpeedRotation *  Time.deltaTime);
    }
}
