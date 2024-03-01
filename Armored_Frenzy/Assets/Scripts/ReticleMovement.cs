using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public enum Controls { Inverted, Standard}

public class ReticleMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    
    InputAction MoveReticalPosition;
    InputAction ReticleSpeed;

    [SerializeField]
    public Vector3 Direction;

    [SerializeField]
    float Speed;

    public bool Move;

    public float speed { 
        get
        {
            return Speed;
        }
        private set
        {
            speed = value;
        }
    }

    public bool PlayerControl;

    float MaxSpeed;
   
    Rigidbody rb;

    [SerializeField]
    private Controls controlInput;

    public Vector3 ForwardPosition;

    /*
    [SerializeField]
    private Material NormalMaterial;

    [SerializeField]
    private Material LockedMaterial;

    private Renderer rend;*/

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        MoveReticalPosition = playerControls.NewPlayer.Move;
        MoveReticalPosition.Enable();

        ReticleSpeed = playerControls.NewPlayer.Acceleration;
        ReticleSpeed.Enable();
        /*
        MoveRetical = playerControls.Player.Rotate;
        MoveRetical.Enable();

        ResetRetical = playerControls.Player.ResetReticle;
        ResetRetical.Enable();*/
    }

    private void OnDisable()
    {
        MoveReticalPosition.Disable();
        ReticleSpeed.Disable();
        /*
        MoveRetical.Disable();
        ResetRetical.Disable();*/
    }


    // Start is called before the first frame update
    void Start()
    {
        if(Speed == 0)
        {
            Speed = 130;
        }

        MaxSpeed = Speed;

        Move = false;

        PlayerControl = true;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(PlayerControl)
        {
            MoveReticle();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
       
    }

    private void MoveReticle()
    {
        var move = MoveReticalPosition.ReadValue<Vector3>();

        Vector3 Rot = new Vector3(-move.y, move.x, 0);
        
        
        if(Rot.y == 0 && Rot.x == 0)
        {
            //Help from https://forum.unity.com/threads/how-to-lerp-rotation.978078/ and chat.gbt
            var currentPos = transform.eulerAngles;
            var current = Quaternion.Euler(currentPos.x, currentPos.y, currentPos.z);
            var reset = Quaternion.Euler(currentPos.x, currentPos.y, 0f);
            float t = 0.1f;
            transform.rotation = Quaternion.Lerp(current, reset, t);
        }

        switch (controlInput)
        { 
            case Controls.Inverted:
                Direction.x = move.x;
                Direction.y = -move.y;
                break;
            case Controls.Standard:
                Direction.x = move.x;
                Direction.y = move.y;
                break;
        }


        if(ReticleSpeed.IsPressed())
        {
            Move = true; 
            Direction.z = 1;
            /*if(Speed < MaxSpeed)
            {
                Speed += 1;
            }*/
        }
        else
        {
            Direction.z = 0;
            Move = false;
            Speed = MaxSpeed;
            /*if(Speed > 0)
            {
                Speed -= 1;
            }*/
        }

        transform.Rotate(Rot * 20 * Time.deltaTime);

        rb.velocity = transform.rotation * Direction * Speed;

        ForwardPosition = transform.TransformDirection(Vector3.forward);

        Debug.DrawRay(transform.position, ForwardPosition, Color.yellow);
    }

    public void IncreaseSpeedWithBoost(float sp)
    {
        if (sp != 0 && ReticleSpeed.IsPressed()) //Go fast until button is released
        {
            Speed += sp;
        }
    }

    /* might need this 
    private void ResetPosition()
    {
        if (ResetRetical.IsPressed())
        {
            transform.localPosition = new Vector3(0, 2, transform.localPosition.z);
        }
    }*/

   
}
