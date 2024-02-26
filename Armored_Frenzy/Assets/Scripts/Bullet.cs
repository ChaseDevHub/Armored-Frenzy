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

    private Rigidbody rb;

    public BulletSide side;

    private Player player;

    private float BulletSpeed;
 
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

        if (player.Speed > player.maxspeed) //if there is a boost
        {
            BulletSpeed = player.Speed * 3;
        }
        else
        {
            BulletSpeed = player.maxspeed * 3;
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

    //Help from chat.gpt
    public void FireFrom(Transform firePoint)
    {
        transform.position = firePoint.position;
        StartLocation = transform.position;
    }

    public void MoveBullet()
    {
        rb.velocity = (Vector3.forward * BulletSpeed);
        //Aim.GetComponent<ReticleMovement>().AimPosition.transform.position;
        /*
         Vector3 targetPosition = Aim.GetComponent<ReticleMovement>().AimPosition.transform.position; //Aim.transform.position;

         var dir = (targetPosition - transform.position).normalized;


         if (Aim.GetComponent<ReticleMovement>().AimPosition.gameObject.CompareTag("GuidePipe") || Aim.GetComponent<ReticleMovement>().AimPosition.gameObject.CompareTag("Track"))
         {
             rb.velocity = Aim.transform.forward * BulletSpeed;
             StartCoroutine(ResetBulletTimer(5));
         }
         else
         {
             rb.velocity = (dir * BulletSpeed);
             StartCoroutine(ResetBulletTimer(2));
         }
         */
        //direction = Aim.transform.forward; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Track") || collision.gameObject.CompareTag("DestroyableObject"))
        {
            ResetBullet();
            this.gameObject.SetActive(false);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PowerUp"))
        {
            ResetBullet();
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator ResetBulletTimer(int time)
    {
        yield return new WaitForSeconds(time);
        ResetBullet();
        this.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private void ResetBullet()
    {
        transform.position = StartLocation;
    }
}
