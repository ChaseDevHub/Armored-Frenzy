using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerTest : Entity
{
    #region Input Fields
    private PlayerControls playerControls;
    InputAction ActivateBoost;
    internal InputAction LeftRotation;
    internal InputAction RightRotation;
    
    internal InputAction Gas;

    #endregion

    [SerializeField]
    private float MaxSpeed;

    public float maxspeed
    {
        get
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

    

    Rigidbody rb;

    public bool PlayerInControl { get; private set; }

    [SerializeField]
    private int ResetTimer;

    

    

    

    [SerializeField]
    float RotationReset;

    [SerializeField]
    private GameObject ParticleEffect;

    private ReticleTest reticle;

    [SerializeField]
    private int SetEnergy; //Can set the Energy in the inspector for however much Energy the player has
    public int Energy { get; set; }

    [SerializeField]
    float RotationAmount;

    public bool HasRotated { get; private set; }

    Vector3 pos = Vector3.zero;

    
    

    #region InputSetUp
    private void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Enable();

        playerControls.NewPlayer.Shoot.performed += ShootWeapon;

        if (SetEnergy == 0)
        {
            SetEnergy = 10;
        }

        Energy = SetEnergy;
    }

    private void OnEnable()
    {
        ActivateBoost = playerControls.NewPlayer.Boost;
        ActivateBoost.Enable();

        LeftRotation = playerControls.NewPlayer.LeftRot;
        LeftRotation.Enable();

        RightRotation = playerControls.NewPlayer.RightRot;
        RightRotation.Enable();

        Gas = playerControls.NewPlayer.Acceleration;
        Gas.Enable();

    }

    private void OnDisable()
    {
        playerControls.NewPlayer.Shoot.performed -= ShootWeapon;
        Gas.Disable();
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Inventory[0] = null;
        
        rb = GetComponent<Rigidbody>();
        PlayerInControl = true;
        

       

        ParticleEffect.SetActive(false);

        if (reticle == null)
        {
            reticle = GameObject.Find("ReticleUI").GetComponent<ReticleTest>();
        }

        if (RotationAmount == 0)
        {
            RotationAmount = 45;
        }

        HasRotated = false;

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        if (Energy > 0 )
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
        else if (Energy == 0)
        {
            UIPlayer.state = PlayerState.Lose;
        }

        reticle.PlayerControl = PlayerInControl;
    }

    private void FollowReticle()
    {
        /*
        Vector3 dir = (reticle.transform.localPosition - rb.position).normalized;
        Vector3 velocitydir = dir;

        rb.velocity = velocitydir * reticle.DefaultSpeed;
        */

        //have it where player follows reticle UI
       
        RotatePlayerOnZAxis();

        //VisualEffect();
    }

    private void RotatePlayerOnZAxis()
    {
        reticle.IncreaseSharpTurn(this);
        if (LeftRotation.IsPressed())
        {
            pos.z = RotationAmount;
        }
        else if (RightRotation.IsPressed())
        {
            pos.z = -RotationAmount;
        }

        if (LeftRotation.IsPressed() || RightRotation.IsPressed())
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
        if (!reticle.Move)
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
        /*
        bm[0].Shoot();
        bm[1].Shoot();
        */
    }

    private void UseBoost()
    {
        if (ActivateBoost.IsPressed() && Inventory[0] != null) //Can only use boost if player has an item in their inventory
        {
            //reticle.IncreaseSpeedWithBoost(20);
            Inventory[0] = null;
            Energy -= 1;
        }
    }

    IEnumerator TrackCollisionCooldown(int timer)
    {
        yield return new WaitForSeconds(timer);
       
        StopAllCoroutines();
    }

    IEnumerator ResetPlayerControl(int timer)
    {
        yield return new WaitForSeconds(timer);

        PlayerInControl = true;
        //Push out

        transform.rotation = Quaternion.Euler(transform.rotation.x, RotationReset, transform.rotation.z);
        reticle.transform.rotation = Quaternion.Euler(transform.rotation.x, RotationReset, transform.rotation.z);

       
        StopAllCoroutines();
    }

    public void IncreaseSpeedWithBoost(float sp)
    {
        if (sp != 0 && Gas.IsPressed()) //Go fast until button is released
        {
            Speed += sp;
        }
    }





}