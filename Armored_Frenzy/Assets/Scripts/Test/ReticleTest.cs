using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ReticleTest : MonoBehaviour
{
    private PlayerControls playerControls;

    InputAction MoveReticalPosition;
    
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

    [SerializeField]
    private Controls controlInput;
   
    private Vector3 ReticlePosition;


    private void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Enable();
        playerControls.NewPlayer.ResetReticle.performed += ResetRetPosition;
    }

    private void OnEnable()
    {
        MoveReticalPosition = playerControls.NewPlayer.Move;
        MoveReticalPosition.Enable();
    }

    private void OnDisable()
    {
        MoveReticalPosition.Disable();

        playerControls.NewPlayer.ResetReticle.performed -= ResetRetPosition;

    }


    // Start is called before the first frame update
    void Start()
    {
        controlInput = UIMenu.control;

        if (DefaultSpeed == 0)
        {
            DefaultSpeed = 12;
            
        }
        Speed = 5;

        Move = false;

        PlayerControl = true;

        ReticlePosition = transform.position; 

        if (RotateAngleAddition == 0)
        {
            RotateAngleAddition = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (PlayerControl)
        {
            MoveReticle();
        }
        else
        {
            Speed = 0;
            this.transform.position = ReticlePosition;
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
        Direction.z = 0;
        
        transform.Translate(Direction * Speed * Time.deltaTime);
 


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

   


    private void ResetRetPosition(InputAction.CallbackContext callback)
    {
        this.transform.position = ReticlePosition;
        
    }
}


