using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Pool;
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

    public int DamagePoint { get; private set; }

    private void Awake()
    {
        Aim = GameObject.Find("Aim").transform;
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
        direction = Aim.transform.forward;
        //transform.position += direction * 2;
        rb.velocity = direction * BulletSpeed;
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
