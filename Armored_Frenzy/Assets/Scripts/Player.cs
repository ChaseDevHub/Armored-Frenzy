using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class Player : Entity
{
    #region Input Fields
    private PlayerControls playerControls;
    InputAction ActivateBoost;
    internal InputAction LeftRotation;
    internal InputAction RightRotation;
   
    
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
    
    private bool ShieldActive;

    Rigidbody rb;

    public bool PlayerInControl { get; private set; }

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
    public int Energy { get; set; }

    [SerializeField]
    float RotationAmount;

    public bool HasRotated { get; private set; }

    Vector3 pos = Vector3.zero;

    private Vector3 GuideRingsLocation;

    float CompareSpeed;

    [SerializeField]
    Audio[] GameAudio;

    bool ProtectPlayer;

    #region InputSetUp
    private void Awake()
    {
        GameAudio = new Audio[2]; 
        playerControls = new PlayerControls();

        playerControls.Enable();

        playerControls.NewPlayer.Shoot.performed += ShootWeapon;

        if (SetEnergy == 0)
        {
            SetEnergy = 10;
        }

        Energy = SetEnergy;

        if (GameAudio[0] == null)
        {
            GameAudio[0] = GameObject.Find("BlastSound").GetComponent<Audio>();
        }
        if (GameAudio[1] == null)
        {
            GameAudio[1] = GameObject.Find("BoostSound").GetComponent<Audio>();
        }
    }

    private void OnEnable()
    {
        ActivateBoost = playerControls.NewPlayer.Boost;
        ActivateBoost.Enable();

        LeftRotation = playerControls.NewPlayer.LeftRot;
        LeftRotation.Enable();

        RightRotation = playerControls.NewPlayer.RightRot;
        RightRotation.Enable();
    }

    private void OnDisable()
    {
        playerControls.NewPlayer.Shoot.performed -= ShootWeapon;
        /*
        ActivateBoost.Disable();
        LeftRotation.Disable();
        RightRotation.Disable();*/
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

        HasRotated = false;

        GuideRingsLocation = GuidePipe.transform.position;
        CompareSpeed = reticle.DefaultSpeed / 2;

        ProtectPlayer = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(HitTrack)
        {
            StartCoroutine(TrackCollisionCooldown(5));
        }

        if(Energy > 0 && UIPlayer.state == PlayerState.Active)
        {
            if (PlayerInControl)
            {
                FollowReticle();

                UseBoost();

            }
            else
            {
                rb.velocity = Vector3.zero;
                transform.Rotate(new Vector3(0, 0, 360) * Time.fixedDeltaTime / 3);
                StartCoroutine(ResetPlayerControl(ResetTimer));
            }
        }
        else if(Energy == 0)
        {
            UIPlayer.state = PlayerState.Lose;
        }

        ActivateShield();
        
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
        GuideRingsLocation = GuidePipe.transform.position;

        reticle.PlayerControl = PlayerInControl;
    }

    private void FollowReticle()
    {
        Vector3 dir = (reticle.transform.localPosition - rb.position).normalized;
        Vector3 velocitydir = dir;
        
        if (reticle.Move)
        {          
            
            rb.velocity = velocitydir * reticle.speed;
            
           
        }
       
        Vector3 rot = Quaternion.LookRotation(reticle.transform.localPosition - transform.position).eulerAngles;
        rot.z = pos.z;

        var currentPos = transform.eulerAngles;
        var current = Quaternion.Euler(currentPos.x, currentPos.y, currentPos.z);
        var next = Quaternion.Euler(rot.x, rot.y, rot.z);
        float t = 0.1f;
        
        transform.rotation = Quaternion.Lerp(current, next, t);

        RotatePlayerOnZAxis();

        VisualEffect();
    }

    private void RotatePlayerOnZAxis()
    {
        
        if (LeftRotation.IsPressed())
        {   
            pos.z = RotationAmount;
        }
        else if (RightRotation.IsPressed())
        {
            pos.z = -RotationAmount;
        }

        if(LeftRotation.IsPressed() || RightRotation.IsPressed())
        {
            pos.z = Mathf.Clamp(pos.z, -RotationAmount, RotationAmount);
            HasRotated = true;
        }
        else
        {
            pos.z = 0;
            HasRotated = false;
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

    private void ShootWeapon(InputAction.CallbackContext callback)
    {
        if (UIPlayer.state == PlayerState.Active)
        {
            GameAudio[0].PlayStart();
            bm[0].Shoot();
            bm[1].Shoot();
        }
        
    }

    private void UseBoost()
    {
        if(ActivateBoost.IsPressed() && Inventory[0] != null) //Can only use boost if player has an item in their inventory
        {
            GameAudio[1].PlayStart();
            reticle.IncreaseSpeedWithBoost(20);
            Inventory[0] = null;
            Energy -= 1;
        }
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
        
        transform.rotation = Quaternion.Euler(transform.rotation.x, RotationReset, transform.rotation.z);
        transform.position = new Vector3(GuideRingsLocation.x, GuideRingsLocation.y, GuideRingsLocation.z + 15);
        
        reticle.RespawnPosition();

        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Track") && HitTrack == false)
        {
            PlayerInControl = false;
            HitTrack = true;
            Energy -= 1;
            ProtectPlayer = true;
        }
        else if(collision.gameObject.CompareTag("DestroyableObject") && !ProtectPlayer)
        {
            HitTrack = true;
            PlayerInControl = false;
            Energy -= 1;
            ProtectPlayer = true;
        }
    }

    private void ActivateShield()
    {
        if(ProtectPlayer)
        {
            //ShieldActive = true;
            StartCoroutine(DamageResistance());
        }
        
    }

    IEnumerator DamageResistance()
    {
        yield return new WaitForSeconds(3.5f);
        ProtectPlayer = false;
        StopAllCoroutines();
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
            //other.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("FinishLine"))
        {
            UIPlayer.state = PlayerState.Win;
        }
    }

    public void StopPlayer()
    {
        rb.velocity = Vector3.zero;
        reticle.StopReticle();
    }
}
