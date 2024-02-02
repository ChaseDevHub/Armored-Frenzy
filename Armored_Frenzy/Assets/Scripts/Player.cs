using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    #region Input Fields
    private PlayerControls playerControls;
    InputAction MovePlayer;
    InputAction StopPlayer;
    InputAction RotateDirection;
    InputAction ActivateBoost;
    InputAction ActivateSpecialItem;
    InputAction ActivateShoot;
    #endregion

    [SerializeField]
    private float MaxSpeed;
    [SerializeField]
    private int BoostTimer;

    private bool BoostActive;

    Rigidbody rb;

    bool PlayerInControl;

    [SerializeField]
    private int ResetTimer;

    bool HitTrack;

    #region InputSetUp
    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        MovePlayer = playerControls.Player.Gas;
        MovePlayer.Enable();

        StopPlayer = playerControls.Player.Break;
        StopPlayer.Enable();

        RotateDirection = playerControls.Player.Move; //for rotation, not actual movement
        RotateDirection.Enable();

        ActivateBoost = playerControls.Player.Boost;
        ActivateBoost.Enable();

        ActivateSpecialItem = playerControls.Player.PowerUp;
        ActivateSpecialItem.Enable();

        ActivateShoot= playerControls.Player.Shoot;
        ActivateShoot.Enable();

    }

    private void OnDisable()
    {
        MovePlayer.Disable();
        StopPlayer.Disable();
        RotateDirection.Disable();
        ActivateBoost.Disable();
        ActivateSpecialItem.Disable();
        ActivateShoot.Disable();
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Inventory[0] = null;
        BoostActive = false;
        rb = GetComponent<Rigidbody>();
        PlayerInControl = true;
        HitTrack= false;
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
            Move();
            UseBoost();
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, 360) * Time.fixedDeltaTime / 3);
            StartCoroutine(ResetPlayerControl(ResetTimer));            
        }
        
    }

    private void Move()
    {
        var rotation = RotateDirection.ReadValue<Vector3>();
        var speed = MovePlayer.ReadValue<float>();

        Direction = Vector3.forward;
        
        Rotation.x = -rotation.y;
        Rotation.y = rotation.x;

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

        transform.Rotate(Rotation * Speed * Time.deltaTime);
        
        rb.velocity = transform.rotation * Direction * Speed; //Help from https://gamedev.stackexchange.com/questions/189313/how-to-do-rigidbody-movement-relative-to-player-rotation-in-unity-c

      
    }

    private void UseBoost()
    {
        if (Inventory[0] != null && ActivateBoost.IsPressed())
        {
            Speed = Speed + Boost;
            Inventory[0] = null;
            BoostActive = true;
            StartCoroutine(BoostCountdown(BoostTimer));
        }
    }

    IEnumerator BoostCountdown(int timer)
    {
        yield return new WaitForSeconds(timer);
        BoostActive= false;
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

        transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PowerUp") && Inventory[0] == null)
        {
            Inventory[0] = other.gameObject;
            other.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Track") && HitTrack == false)
        {
            PlayerInControl = false;
            HitTrack = true;
        }
    }
}
