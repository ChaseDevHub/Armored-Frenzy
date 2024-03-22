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
    internal InputAction ReticleSpeed;
    InputAction ResetPosition;

    [SerializeField]
    public Vector3 Direction;

    [SerializeField]
    public float DefaultSpeed;

    [SerializeField]
    private float Speed;

    public bool Move;


    public float RotateAngleAddition;

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
        controlInput = UIMenu.control;

        if(DefaultSpeed == 0)
        {
            DefaultSpeed = 40;
        }

        MaxSpeed = DefaultSpeed;

        Speed = 0;

        Move = false;

        PlayerControl = true;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        player = GameObject.Find("Player").GetComponent<Player>();
        ReticlePosition = GameObject.Find("ReticlePosition").GetComponent<Transform>();

        this.transform.position = ReticlePosition.position;

        if(RotateAngleAddition == 0)
        {
            RotateAngleAddition = 2;
        }
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
            Speed = 0;
            this.transform.position = ReticlePosition.position;
        }
       
    }
    internal bool ResetRetPos = false;

    private void MoveReticle()
    {
        var move = MoveReticalPosition.ReadValue<Vector3>();
        Direction.z = -ReticleSpeed.ReadValue<float>();
        
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

        if (ReticleSpeed.IsPressed())
        {
            ResetRetPos = false;
            Move = true;
            UIPlayer.StartTimer = true;
            if (Speed < MaxSpeed)
            {
                Speed = Speed + 1;
               
            }
        }
        else
        {
            Move = false;
            if (Speed > 0 && Speed != 1)
            {
                Speed = Speed - 1;
                ResetRetPos = false;
                //this.transform.position = ReticlePosition.position; //Temp
            }
            else if(Speed > 0 && Speed == 1)
            {
                ResetRetPos = true;
                Speed = Speed - 1;
            }
            
  
        }
      
        ResetRetPosition();
        IncreaseSharpTurn();

        if(ResetRetPos)
        {
            this.transform.position = ReticlePosition.position; //Temp
            ResetRetPos = false;
        }

        //help with modifying with Chat.gpt
        Vector3 velocity = new Vector3(Direction.x, Direction.y, Direction.z) * Speed;
        Vector3 rotate = new Vector3(Direction.x, Direction.y, Direction.z) * DefaultSpeed;
        
        if(!Move)
        {
            rb.velocity = transform.rotation * rotate;
        }
        else
        {
            rb.velocity = transform.rotation * velocity;
        }
        
        

        //Visual for debug
        ForwardPosition = transform.TransformDirection(Vector3.back);
        //Debug.DrawRay(transform.position, ForwardPosition, Color.yellow);
    }

    private void IncreaseSharpTurn()
    {
        if (player.HasRotated && player.LeftRotation.IsPressed() && Direction.x == 1)
        {
            Direction.x = RotateAngleAddition;
        }
        else if (player.HasRotated && player.RightRotation.IsPressed() && Direction.x == -1)
        {
            Direction.x = -RotateAngleAddition;

        }
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
