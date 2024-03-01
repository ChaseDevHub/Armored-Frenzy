using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public enum BulletSide { left, right };
public class Bullet : MonoBehaviour
{
    private GameObject BulletMachine;
    private Vector3 StartLocation;
    
    private GameObject Reticle;

    private Rigidbody rb;

    public BulletSide side;

    private Player player;

    private float BulletSpeed;

    private bool HitReticle;

    public int DamagePoint { get; private set; }

    private void Awake()
    {
        Reticle = GameObject.Find("Reticle");
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

        HitReticle = false;
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
        Vector3 forward = Reticle.transform.localPosition - transform.position;
        Vector3 straight = Reticle.GetComponent<ReticleMovement>().ForwardPosition;

        Debug.DrawRay(transform.position, forward, Color.green);

        if(!HitReticle)
        {
            rb.velocity = forward * BulletSpeed * Time.fixedDeltaTime;
        }
        else
        {
            rb.velocity = straight * BulletSpeed * 5 * Time.fixedDeltaTime;
        }

        
        StartCoroutine(ResetBulletTimer(2));
        
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
        if(other.gameObject.CompareTag("Reticle"))
        {
            HitReticle = true;
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
        HitReticle= false;
        transform.position = StartLocation;
    }
}
