using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Controls { Inverted, Standard}

public class ReticleMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    
    InputAction MoveReticalPosition;
    InputAction ReticleSpeed;

    [SerializeField]
    Vector3 Direction;

    [SerializeField]
    float Speed;

    float MaxSpeed;
    float DefaultSpeed;

    Rigidbody rb;

    [SerializeField]
    private Controls controlInput;

    /*
    InputAction MoveRetical;
    InputAction ResetRetical;

    public Vector2 Movement;
    public float MoveSpeed;

    public Transform AimPosition;

    private GameObject player;

    [SerializeField]
    private float Up;
    [SerializeField]
    private float Down;
    [SerializeField]
    private float Left;
    [SerializeField]
    private float Right;

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

        MaxSpeed = Speed + 20;

        //DefaultSpeed = Speed;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        #region OLDStart
        /*
        AimPosition = null;
        if (MoveSpeed == 0)
        {
            MoveSpeed = 15;
        }

        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        rend = GetComponent<MeshRenderer>();  

        rend.material = NormalMaterial;
        */
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveReticle();
        //MoveRet();
        //LookAhead();
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
                Direction.x = move.x;
                Direction.y = move.y;
                break;
        }

        if(ReticleSpeed.IsPressed())
        {
            Direction.z = 1;
            if(Speed < MaxSpeed)
            {
                Speed += 1;
            }
        }
        else
        {
            Direction.z = 0;
            if(Speed > 0)
            {
                Speed -= 1;
            }
        }

        rb.velocity = Direction * Speed * Time.fixedDeltaTime;
    }

    #region OLD
    /*
    private void MoveRet()
    {
        //limit how much it can move
        var move = MoveRetical.ReadValue<Vector3>();

        Movement.x = move.x;
        Movement.y = move.y;

        transform.Translate(Movement * MoveSpeed * Time.deltaTime);

        //help from https://discussions.unity.com/t/restricting-movement-with-mathf-clamp/133376/2
        Vector3 pos = transform.localPosition;
        pos.y = Mathf.Clamp(pos.y, Down, Up);
        pos.x = Mathf.Clamp(pos.x, Left, Right);
        transform.localPosition = pos;

        ResetPosition();
    }

    private void LookAhead()
    {

        //Help with getting aim to look forward based on direction: https://discussions.unity.com/t/casting-ray-forward-from-transform-position/48120/2
        Vector3 move = transform.TransformDirection(Vector3.forward);

        if (Movement.x > 0)
        {
            move.x = 1;
        }
        if (Movement.x < 0)
        {
            move.x = -1;
        }
        if (Movement.y > 0)
        {
            move.y = 1;
        }
        if (Movement.y < 0)
        {
            move.y = -1;
        }

        move.z = 1;

        Vector3 dir = move * 2;

        //Is there a way to use lerp here?
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit))
        {
            //This is used to see the line that it is displaying correct 
            Debug.DrawRay(transform.position, dir * 100, Color.yellow);

            if (hit.collider)
            {
                AimPosition = hit.collider.gameObject.transform;
            }

            if (hit.collider.gameObject.CompareTag("GuidePipe") || hit.collider.gameObject.CompareTag("Track"))
            {
                ChangeMaterial(false);
            }
            else
            {
                ChangeMaterial(true);
            }    
        }
    }

    private void ResetPosition()
    {
        if (ResetRetical.IsPressed())
        {
            transform.localPosition = new Vector3(0, 2, transform.localPosition.z);
        }
    }
    
    public void ChangeMaterial(bool LockedIn)
    {
        if(LockedIn)
        {
            rend.material = LockedMaterial;
        }
        else
        {
            rend.material = NormalMaterial;
        }
    }
    */
    #endregion
}
