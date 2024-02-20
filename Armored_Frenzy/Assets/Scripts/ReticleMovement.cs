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
    public float RetSpeed;

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
        if (RetSpeed == 0)
        {
            RetSpeed = 10;
        }

        if (player == null)
        {
            player = GameObject.Find("Player");
        }

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

        transform.Translate(Movement * RetSpeed * Time.deltaTime);

        //help from https://discussions.unity.com/t/restricting-movement-with-mathf-clamp/133376/2
        Vector3 pos = transform.localPosition;
        pos.y = Mathf.Clamp(pos.y, Down, Up);
        pos.x = Mathf.Clamp(pos.x, Left, Right);
        transform.localPosition = pos;

        ResetPosition();
    }

    private void LookAhead()
    {
        //Should this only look forward from a certain distance?

        Vector3 move = Vector3.forward;

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

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit))
        {
            Debug.DrawRay(transform.position, dir * 100, Color.yellow);


            if (hit.collider)
            {
                AimPosition = hit.collider.gameObject.transform;
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
}
