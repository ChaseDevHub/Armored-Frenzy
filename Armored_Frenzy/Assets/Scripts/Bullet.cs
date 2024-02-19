using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public enum BulletSide { left, right };
public class Bullet : MonoBehaviour
{
    private GameObject BulletMachine;
    private Vector3 StartLocation;
    private Transform Aim;
    private Vector3 direction;
   

    private Rigidbody rb;

    public BulletSide side;

    private Player player;

    private float BulletSpeed;

    float timeRemaining = 1;
    float timeDefault;

    public int DamagePoint { get; private set; }

    private void Awake()
    {
        Aim = GameObject.Find("Reticle").transform;
        if(player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }
    }

    private void Start()
    {
        switch (side)
        {
            case BulletSide.left:
                BulletMachine = GameObject.Find("BulletSlotLeft");
                break; 
            case BulletSide.right:
                BulletMachine = GameObject.Find("BulletSlotRight");
                break;
        }

       
        if(rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        DamagePoint = 2;

        if(player.Speed > player.maxspeed) //if there is a boost
        {
            BulletSpeed = player.Speed * 3;
        }
        else
        {
            BulletSpeed = player.maxspeed * 3;
        }

        
        timeDefault = timeRemaining;
    }

    private void FixedUpdate()
    {
        if (enabled)
        {
            MoveBullet();
        }

        StartLocation = BulletMachine.transform.position;
    }

    //Help from chat.gpt
    public void FireFrom(Transform firePoint)
    {
        transform.position = firePoint.position;
        StartLocation = transform.position;
    }

    public void MoveBullet()
    {
        /*
         Vector3 targetPosition = Aim.transform.position;

        var dir = (targetPosition - transform.position);

        if (!hitReticle)
        {
            rb.velocity = dir * BulletSpeed;
            //rb.AddForce(dir* BulletSpeed);

        }
        else
        {
            rb.velocity = Aim.transform.forward * BulletSpeed;
            //rb.AddForce(Aim.transform.forward * BulletSpeed);
        }
         */

        //Aim.GetComponent<ReticleMovement>().AimPosition.transform.position;
        Vector3 targetPosition = Aim.transform.position;

        var dir = (targetPosition - transform.position).normalized;

        rb.velocity = (dir * BulletSpeed);

        //direction = Aim.transform.forward;

        StartCoroutine(ResetBulletTimer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Track") || collision.gameObject.CompareTag("DestroyableObject"))
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
