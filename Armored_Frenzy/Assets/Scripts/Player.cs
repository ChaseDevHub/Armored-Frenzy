using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using Debug = UnityEngine.Debug;

public class Player : Entity
{
    #region Input Fields
    private PlayerControls playerControls;
    InputAction ActivateShoot;
    InputAction ActivateBoost;
   
    
    #endregion

    [SerializeField]
    private float MaxSpeed;

    public float maxspeed { get
        {
            return MaxSpeed;
        }
        private set
        {
            maxspeed = value;
        }
    
    }

    [SerializeField]
    private int BoostTimer;

    [SerializeField]
    private int ShieldTimer;

    
    //private bool BoostActive;
    
    private bool ShieldActive;

    Rigidbody rb;

    bool PlayerInControl;

    [SerializeField]
    private int ResetTimer;

    bool HitTrack;

    public BulletMachine[] bm;

    private GameObject Shield;

    [SerializeField]
    private GameObject GuidePipe;

    [SerializeField]
    float RotationReset;

    [SerializeField]
    private GameObject ParticleEffect;

    private ReticleMovement reticle;

    [SerializeField]
    private int SetEnergy; //Can set the Energy in the inspector for however much Energy the player has
    public int Energy { get; private set; }

    [SerializeField]
    float RotationAmount;

    float timer = 0;

    Vector3 pos = Vector3.zero;

    #region InputSetUp
    private void Awake()
    {
        playerControls = new PlayerControls();
        if (SetEnergy == 0)
        {
            SetEnergy = 10;
        }

        Energy = SetEnergy;
    }

    private void OnEnable()
    {
        ActivateShoot= playerControls.NewPlayer.Shoot;
        ActivateShoot.Enable();

        ActivateBoost = playerControls.NewPlayer.Boost;
        ActivateBoost.Enable();
    }

    private void OnDisable()
    {
        ActivateShoot.Disable();
        ActivateBoost.Disable();
        
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Inventory[0] = null;
        //BoostActive = false;
        ShieldActive = false;
        rb = GetComponent<Rigidbody>();
        PlayerInControl = true;
        HitTrack= false;
        Shield = GameObject.Find("Shield");
        Shield.SetActive(false);
        
        if(GuidePipe == null)
        {
            GuidePipe = GameObject.FindGameObjectWithTag("GuidePipe");
        }

        ParticleEffect.SetActive(false);

        if(reticle == null)
        {
            reticle = GameObject.Find("Reticle").GetComponent<ReticleMovement>();
        }

        if(RotationAmount == 0)
        {
            RotationAmount = 45;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(HitTrack)
        {
            StartCoroutine(TrackCollisionCooldown(10));
        }

        if(PlayerInControl)
        {
            FollowReticle();
            
            UseBoost();
           
            ShootWeapon();
        }
        else
        {
            
            transform.Rotate(new Vector3(0, 0, 360) * Time.fixedDeltaTime / 3);
            StartCoroutine(ResetPlayerControl(ResetTimer));            
        }
        
        if(ShieldActive)
        {
            Shield.SetActive(true);
        }
        else
        {
            Shield.SetActive(false);
        }

        Quaternion guidePipeRotation = GuidePipe.transform.rotation;
        RotationReset = guidePipeRotation.eulerAngles.y;

        reticle.PlayerControl = PlayerInControl;
    }

    private void FollowReticle()
    {
        if(reticle.Move)
        {          
            Vector3 dir = (reticle.transform.localPosition - rb.position).normalized;
            Vector3 velocitydir = dir;

            rb.velocity = velocitydir * reticle.speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }



        //transform.LookAt(reticle.transform.localPosition);
        Vector3 rot = Quaternion.LookRotation(reticle.transform.localPosition - transform.position).eulerAngles;
        rot.z = pos.z;

        var currentPos = transform.eulerAngles;
        var current = Quaternion.Euler(currentPos.x, currentPos.y, currentPos.z);
        var next = Quaternion.Euler(rot.x, rot.y, rot.z);
        float t = 0.1f;
        
        transform.rotation = Quaternion.Lerp(current, next, t);

        //transform.rotation = Quaternion.Euler(rot);

        RotatePlayerOnZAxis();

        VisualEffect();
    }

    private void RotatePlayerOnZAxis()
    {
        //I should instead add them to the input controls
        var rotL = Gamepad.current.leftShoulder;
        var rotR = Gamepad.current.rightShoulder;

        if (rotL.IsPressed())
        {
            pos.z = -RotationAmount;
        }
        else if (rotR.IsPressed())
        {
            pos.z = RotationAmount;
        }


        if(rotL.IsPressed() || rotR.IsPressed())
        {
            pos.z = Mathf.Clamp(pos.z, -RotationAmount, RotationAmount);
        }
        else
        {
            timer = 0;
            pos.z = 0;
        }
    }

    

    private void VisualEffect()
    {
        if(!reticle.Move)
        {
            ParticleEffect.SetActive(false);
        }
        else
        {
            ParticleEffect.SetActive(true);
        }
    }

    private void ShootWeapon()
    {
        if(ActivateShoot.IsPressed())
        {
            bm[0].Shoot();
            bm[1].Shoot();
        }
    }
    
    //Need some changing
    private void UseBoost()
    {
        /*
        if (Inventory[0] != null && ActivateBoost.IsPressed() && Inventory[0].GetComponent<PowerUp>().Ability == PowerName.Boost)
        {
            Speed = Speed + Boost;
            Inventory[0] = null;
            BoostActive = true;
            StartCoroutine(BoostCountdown(BoostTimer));
        }*/

        if(ActivateBoost.IsPressed() && Inventory[0] != null) //Can only use boost if player has an item in their inventory
        {
            reticle.IncreaseSpeedWithBoost(20);
            Inventory[0] = null;
            Energy -= 1;
            Debug.Log("Boost has been pressed");
        }
    }
   
    IEnumerator BoostCountdown(int timer)
    {
        yield return new WaitForSeconds(timer);
        //BoostActive= false;
        StopAllCoroutines();
    }

    IEnumerator ShieldCountdown(int timer)
    {
        yield return new WaitForSeconds(timer);
        ShieldActive = false;
        StopAllCoroutines();
    }

    IEnumerator TrackCollisionCooldown(int timer)
    {
        yield return new WaitForSeconds(timer);
        HitTrack = false;
        StopAllCoroutines();
    }

    IEnumerator ResetPlayerControl(int timer)
    {
        yield return new WaitForSeconds(timer);
        
        PlayerInControl = true;
        //Push out

        //transform.rotation = Quaternion.Euler(transform.rotation.x, RotationReset, transform.rotation.z);
        reticle.transform.rotation = Quaternion.Euler(transform.rotation.x, RotationReset, transform.rotation.z);
        
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Track") && HitTrack == false)
        {
            PlayerInControl = false;
            HitTrack = true;
            Energy -= 1;
        }
        else if(collision.gameObject.CompareTag("DestroyableObject"))
        {
            PlayerInControl = false;
        }

        //Energy -= 1; //should it lose Energy from hitting everything or
                     //only with certain objects?
                     //no, because it brings down the Energy too fast when hitting everything 
                     //Unless that is something we want?
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GuidePipe"))
        {
            GuidePipe = other.gameObject;
        }

        //Passing through object
        if (other.gameObject.CompareTag("PowerUp") && Inventory[0] == null)
        {
            Inventory[0] = other.gameObject;
            other.gameObject.SetActive(false);
        }
    }


}
