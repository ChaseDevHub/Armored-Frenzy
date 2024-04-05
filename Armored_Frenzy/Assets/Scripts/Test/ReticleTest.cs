using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ReticleTest : MonoBehaviour
{
    private PlayerControls playerControls;

    InputAction MoveReticalPosition;
    
    InputAction ResetPosition;

    [SerializeField]
    public Vector3 Direction;

    [SerializeField]
    public float DefaultSpeed;

    [SerializeField]
    private float Speed;

    public bool Move;


    public float RotateAngleAddition;

    public float speed
    {
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

    //Rigidbody rb;

    [SerializeField]
    private Controls controlInput;

    public Vector3 ForwardPosition;

    //private Player player;

    private Transform ReticlePosition;

    private Rigidbody rb;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        MoveReticalPosition = playerControls.NewPlayer.Move;
        MoveReticalPosition.Enable();

       
        ResetPosition = playerControls.NewPlayer.ResetReticle;
        ResetPosition.Enable();
     
    }

    private void OnDisable()
    {
        MoveReticalPosition.Disable();
        
        ResetPosition.Disable();
        
    }


    // Start is called before the first frame update
    void Start()
    {
        controlInput = UIMenu.control;

        if (DefaultSpeed == 0)
        {
            DefaultSpeed = 40;
        }

        if(rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        MaxSpeed = DefaultSpeed;

        Speed = 0;

        Move = false;

        PlayerControl = true;

        ReticlePosition = GameObject.Find("ReticlePosition").GetComponent<Transform>();

        this.transform.position = ReticlePosition.position;

        if (RotateAngleAddition == 0)
        {
            RotateAngleAddition = 2;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.LookAt(player.transform.localPosition);

        if (PlayerControl)
        {
            MoveReticle();
        }
        else
        {
            //rb.velocity = Vector3.zero;
            Speed = 0;
            this.transform.position = ReticlePosition.position;
        }

    }
    internal bool ResetRetPos = false;

    private void MoveReticle()
    {
        var move = MoveReticalPosition.ReadValue<Vector3>();
       

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
        

        ResetRetPosition();
        //IncreaseSharpTurn();

        

        //help with modifying with Chat.gpt
        //Vector3 velocity = new Vector3(Direction.x, Direction.y, Direction.z) * Speed;
        Vector3 rotate = new Vector3(Direction.x, Direction.y, Direction.z) * DefaultSpeed;
        rb.velocity = transform.rotation * rotate;

        //Visual for debug
        ForwardPosition = transform.TransformDirection(Vector3.back);
        //Debug.DrawRay(transform.position, ForwardPosition, Color.yellow);
    }

    public void IncreaseSharpTurn(PlayerTest player)
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

   


    private void ResetRetPosition()
    {
        if (ResetPosition.IsPressed())
        {
            this.transform.position = ReticlePosition.position;
        }
    }
}


