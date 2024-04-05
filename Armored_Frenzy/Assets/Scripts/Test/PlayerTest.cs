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

    private bool ShieldActive;

    Rigidbody rb;

    public bool PlayerInControl { get; private set; }

    [SerializeField]
    private int ResetTimer;

    bool HitTrack;

    public BulletMachine[] bm;

    //private GameObject Shield;

    [SerializeField]
    private GameObject GuidePipe;

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

    private Vector3 GuideRingsLocation;

    float CompareSpeed;

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
        //BoostActive = false;
        ShieldActive = false;
        rb = GetComponent<Rigidbody>();
        PlayerInControl = true;
        HitTrack = false;
        //Shield = GameObject.Find("Shield");
        //Shield.SetActive(false);

        if (GuidePipe == null)
        {
            GuidePipe = GameObject.FindGameObjectWithTag("GuidePipe");
        }

        ParticleEffect.SetActive(false);

        if (reticle == null)
        {
            reticle = GameObject.Find("Reticle").GetComponent<ReticleTest>();
        }

        if (RotationAmount == 0)
        {
            RotationAmount = 45;
        }

        HasRotated = false;

        GuideRingsLocation = GuidePipe.transform.position;
        CompareSpeed = reticle.DefaultSpeed / 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (HitTrack)
        {
            StartCoroutine(TrackCollisionCooldown(10));
        }

        if (Energy > 0 )//&& UIPlayer.state == PlayerState.Active)
        {
            if (PlayerInControl)
            {
                //rb.transform.LookAt(reticle.transform.position);

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


        /*
        if (ShieldActive)
        {
            Shield.SetActive(true);
        }
        else
        {
            Shield.SetActive(false);
        }*/

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

            rb.velocity = velocitydir * reticle.DefaultSpeed;


        }
        else
        {
            if (reticle.speed > 0)
            {
                //Speed = Speed - 1;
                rb.velocity = velocitydir * reticle.DefaultSpeed / 2;
            }
            else
            {

                rb.velocity = Vector3.zero;
            }

        }

        //Check later
        Vector3 direction = reticle.transform.position - rb.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float rotationSpeed = 2.0f;

        rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

       
        RotatePlayerOnZAxis();

        VisualEffect();
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
        bm[0].Shoot();
        bm[1].Shoot();
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
        HitTrack = false;
        StopAllCoroutines();
    }

    IEnumerator ResetPlayerControl(int timer)
    {
        yield return new WaitForSeconds(timer);

        PlayerInControl = true;
        //Push out

        transform.rotation = Quaternion.Euler(transform.rotation.x, RotationReset, transform.rotation.z);
        reticle.transform.rotation = Quaternion.Euler(transform.rotation.x, RotationReset, transform.rotation.z);

        //reset back to the previous check point after crashing
        reticle.transform.position = new Vector3(GuideRingsLocation.x, GuideRingsLocation.y, GuideRingsLocation.z + 30);
        transform.position = new Vector3(GuideRingsLocation.x, GuideRingsLocation.y, GuideRingsLocation.z + 30);

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