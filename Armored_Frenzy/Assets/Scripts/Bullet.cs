using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private GameObject BulletMachine;
    private Vector3 StartLocation;
    private Transform Aim;
    private Vector3 direction;

    private Rigidbody rb;

    private void Start()
    {
        Aim = GameObject.Find("Aim").transform;
        BulletMachine = GameObject.Find("BulletSlot");

        transform.position = BulletMachine.transform.position;
        StartLocation = transform.position;
        if(rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (enabled)
        {
            MoveBullet();
        }

        StartLocation = BulletMachine.transform.position;
    }

    private void MoveBullet()
    {
        direction = Aim.transform.forward;
        transform.position += direction;
        StartCoroutine(ResetBulletTimer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Track"))
        {
            ResetBullet();
            this.gameObject.SetActive(false);
        }
        
    }

    IEnumerator ResetBulletTimer()
    {
        yield return new WaitForSeconds(5);
        ResetBullet();
        this.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private void ResetBullet()
    {
        transform.position = StartLocation;
    }
}
