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
    InputAction ResetPosition;

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

    public float DefaultSpeed;

    public bool PlayerControl;

    public float MaxSpeed;
   
    Rigidbody rb;

    [SerializeField]
    private Controls controlInput;

    public Vector3 ForwardPosition;

    private Player player;

    private Transform ReticlePosition;

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

        ResetPosition = playerControls.NewPlayer.ResetReticle;
        ResetPosition.Enable();
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
        ResetPosition.Disable();
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

        DefaultSpeed = Speed;

        Move = false;

        PlayerControl = true;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        player = GameObject.Find("Player").GetComponent<Player>();
        ReticlePosition = GameObject.Find("ReticlePosition").GetComponent<Transform>();

        this.transform.position = ReticlePosition.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(player.transform.localPosition);
        
        if (PlayerControl)
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

        switch (controlInput)
        { 
            case Controls.Inverted:
                Direction.x = -move.x;
                Direction.y = -move.y;
                break;
            case Controls.Standard:
                Direction.x = -move.x;
                Direction.y = move.y;
                break;
        }

        if(ReticleSpeed.IsPressed())
        {
            Move = true; 
            Direction.z = -1;
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

        ResetRetPosition();

        rb.velocity = transform.rotation * Direction * Speed;

        //Visual for debug
        ForwardPosition = transform.TransformDirection(Vector3.back);
        Debug.DrawRay(transform.position, ForwardPosition, Color.yellow);
    }

    public void IncreaseSpeedWithBoost(float sp)
    {
        if (sp != 0 && ReticleSpeed.IsPressed()) //Go fast until button is released
        {
            Speed += sp;
        }
    }

     
    private void ResetRetPosition()
    {
        if(ResetPosition.IsPressed())
        {
            this.transform.position = ReticlePosition.position;
        }
    }


}
