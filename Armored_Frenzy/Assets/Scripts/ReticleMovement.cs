using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReticleMovement : MonoBehaviour
{
    private PlayerControls playerControls;

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

    private Renderer rend;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        MoveRetical = playerControls.Player.Rotate;
        MoveRetical.Enable();

        ResetRetical = playerControls.Player.ResetReticle;
        ResetRetical.Enable();
    }

    private void OnDisable()
    {
        MoveRetical.Disable();
        ResetRetical.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        MoveRet();
        LookAhead();
    }

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
}
