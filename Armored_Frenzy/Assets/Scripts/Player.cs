using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    private PlayerControls playerControls;
    InputAction MovePlayer;
    InputAction StopPlayer;
    InputAction RotateDirection;
    InputAction ActivateBoost;
    InputAction ActivateSpecialItem;
    InputAction ActivateShoot;

    [SerializeField]
    private float MaxSpeed;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        var rotation = RotateDirection.ReadValue<Vector3>();
        var speed = MovePlayer.ReadValue<float>();

        Direction = Vector3.forward;

        Rotation.x = -rotation.y;
        Rotation.y = rotation.x;


        if (MovePlayer.IsPressed() && !StopPlayer.IsPressed())
        {
            if (Speed < MaxSpeed)
            {
                Speed = Speed + speed;
            }

        }
        else if (!MovePlayer.IsPressed() && !StopPlayer.IsPressed())
        {
            if(Speed > 0)
            {
                Speed = Speed - 5.5f * Time.deltaTime;
            }
            
        }
        else if(StopPlayer.IsPressed() && !MovePlayer.IsPressed())
        {
            if (Speed > 0)
            {
                Speed = Speed - 1;
            }
        }

        transform.Rotate(Rotation * Speed * Time.deltaTime);

        /*
        if(rotation.y == 0 && rotation.x == 0)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
        }*/
        
        transform.Translate(Direction * Speed * Time.deltaTime);
    }
}
