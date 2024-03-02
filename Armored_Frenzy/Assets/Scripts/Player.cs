using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
            //transform.position = Vector3.MoveTowards(transform.position, reticle.transform.position, reticle.speed * Time.deltaTime);
            Vector3 dir = (reticle.transform.localPosition - rb.position).normalized;//Vector3.MoveTowards(transform.position, reticle.transform.localPosition, reticle.speed * Time.deltaTime);
            Vector3 velocitydir = dir;

            rb.velocity = velocitydir * reticle.speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
        

        transform.LookAt(reticle.transform.localPosition); //mainly for if object is child

        /*
        var rotL = Gamepad.current.leftShoulder;
        var rotR = Gamepad.current.rightShoulder;

        if (rotL.IsPressed())
        {
            //help from https://discussions.unity.com/t/restricting-movement-with-mathf-clamp/133376/2
            Vector3 pos = transform.rotation.eulerAngles;
            pos.z = Mathf.Clamp(0, 0, -90);
            
            transform.Rotate(pos);
        }
        else if (rotR.IsPressed())
        {
            Vector3 pos = transform.rotation.eulerAngles;
            pos.z = Mathf.Clamp(0, 0, 180);

            transform.Rotate(pos);
        }*/
        
        //Have it where the player is rotation on the z axis but once everything is FIXED

        VisualEffect();
    }

    /*
    private void Move()
    {
        
        var rotation = RotateDirection.ReadValue<Vector3>();
        var speed = MovePlayer.ReadValue<float>();

        Direction = Vector3.forward;
        
        Rotation.x = -rotation.y;
        Rotation.y = rotation.x;
        
        if(Rotation.y != 0)
        {
            Rotation.z = -rotation.x * 2;
        }
        else
        {
            //Help from https://forum.unity.com/threads/how-to-lerp-rotation.978078/ and chat.gbt
            var currentPos = transform.eulerAngles;
            var current = Quaternion.Euler(currentPos.x, currentPos.y, currentPos.z);
            var reset = Quaternion.Euler(currentPos.x, currentPos.y, 0f); 
            float t = 0.1f;
            transform.rotation = Quaternion.Lerp(current, reset, t);
        }

        if (MovePlayer.IsPressed() && !StopPlayer.IsPressed()) //press gas
        {
            if (Speed < MaxSpeed)
            {
                Speed = Speed + speed;
            }
            else if(Speed > MaxSpeed && !BoostActive) 
            {
                Speed = Speed - 1 * Time.deltaTime;
            }
        }
        else if (!MovePlayer.IsPressed() && !StopPlayer.IsPressed() && !BoostActive) //let go gas but not press break
        {
            if(Speed > 0)
            {
                Speed = Speed - 5.5f * Time.deltaTime;
            }
            
        }
        else if(StopPlayer.IsPressed() && !MovePlayer.IsPressed() && !BoostActive) //press break
        {
            if (Speed > 0)
            {
                Speed = Speed - 1;
            }
        }


        transform.Rotate(Rotation * 30 * Time.deltaTime);
        
        rb.velocity = transform.rotation * Direction * Speed; //Help from https://gamedev.stackexchange.com/questions/189313/how-to-do-rigidbody-movement-relative-to-player-rotation-in-unity-c
        

        VisualEffect();
    }*/

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

        //maybe have the rotation reset based on the Z axis of the track marks?
        
        transform.rotation = Quaternion.Euler(transform.rotation.x, RotationReset, transform.rotation.z);
        
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
