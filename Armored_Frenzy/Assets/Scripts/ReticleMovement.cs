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

    public float MaxSpeed;

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

    Rigidbody rb;

    [SerializeField]
    private Controls controlInput;

    public Vector3 ForwardPosition;

    private Player player;

    private Transform ReticlePosition;

    [SerializeField]
    private float timeRemain;
    private float timeDefault;

    private bool ResetSpeedChange;
    internal bool ResetRetPos = false;

    [SerializeField]
    GameObject TrailNormal;
    
    [SerializeField]
    GameObject TrailBoost;

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

        Speed = DefaultSpeed / 2;


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

        timeRemain = 3;
        timeDefault = timeRemain;

        ResetSpeedChange = false;

        if(TrailNormal == null)
        {
            TrailNormal = GameObject.Find("EngineTrailsNormal");
        }
        
        if(TrailBoost == null)
        {
            TrailBoost = GameObject.Find("EngineTrailsBoost");
        }

        TrailBoost.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(player.transform.localPosition);

        if(ResetSpeedChange == true)
        {
            ReturnSpeedToNormal();
        }
        

        if (player.PlayerInControl && UIPlayer.state == PlayerState.Active)
        {
            MoveReticle();
            
        }
        else
        {
            rb.velocity = Vector3.zero;
           
            this.transform.position = ReticlePosition.position;
        }
    }
    
    private void MoveReticle()
    {
        var move = MoveReticalPosition.ReadValue<Vector3>();
        Direction.z = -1;//-ReticleSpeed.ReadValue<float>();
        Move = true;

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

        
        if (ReticleSpeed.IsPressed() || ResetSpeedChange)
        {
            Speed = MaxSpeed;
        }
        else if(!ResetSpeedChange || !ReticleSpeed.IsPressed())
        {
            Speed = DefaultSpeed / 2;
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
        
        rb.velocity = transform.rotation * velocity;

        ForwardPosition = transform.TransformDirection(Vector3.back);
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
        timeRemain = timeDefault;
        SetTrails(false, true);
        MaxSpeed += sp;
        ResetSpeedChange = true;
    }

    private void ReturnSpeedToNormal()
    {
        if (timeRemain > 0)
        {
            timeRemain -= Time.deltaTime;
        }
        else
        {
            SetTrails(true, false);
            ResetSpeedChange = false;
            MaxSpeed = DefaultSpeed;
            timeRemain = timeDefault;
            ResetRetPos = true;
        }
    }

    private void SetTrails(bool normalCondition, bool boostCondition)
    {
        TrailNormal.SetActive(normalCondition);
        TrailBoost.SetActive(boostCondition);
    }
     
    private void ResetRetPosition()
    {
        if(ResetPosition.IsPressed())
        {
            this.transform.position = ReticlePosition.position;
        }
    }

    //Called from Player class when player crashes and resets
    public void RespawnPosition()
    {
        this.transform.position = ReticlePosition.position; 
    }

    public void StopReticle()
    {
        rb.velocity = Vector3.zero;
    }
    

}
