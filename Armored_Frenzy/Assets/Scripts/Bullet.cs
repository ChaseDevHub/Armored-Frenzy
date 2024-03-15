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

    private ReticleMovement reticlemovement;

    private float BulletSpeed;

    private bool HitReticle;

    public int DamagePoint { get; private set; }

    private void Awake()
    {
        Reticle = GameObject.Find("Reticle");
        if(reticlemovement == null)
        {
            reticlemovement = GameObject.Find("Reticle").GetComponent<ReticleMovement>();
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

        if (reticlemovement.DefaultSpeed > reticlemovement.MaxSpeed) //if there is a boost
        {
            BulletSpeed = reticlemovement.speed;
        }
        else
        {
            BulletSpeed = reticlemovement.MaxSpeed * 2;
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
            rb.velocity = forward * BulletSpeed;
        }
        else
        {
            rb.velocity = straight * BulletSpeed * 2;
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
