using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReticleMovement : MonoBehaviour
{
    private PlayerControls playerControls;

    InputAction MoveRetical;

    public Vector2 Movement;
    public float RetSpeed;

    public Transform AimPosition;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        MoveRetical = playerControls.Player.Rotate;
        MoveRetical.Enable();

    }

    private void OnDisable()
    {
        MoveRetical.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        AimPosition = null;
    }

    // Update is called once per frame
    void Update()
    {
        MoveRet();
        LookAhead();
    }

    private void MoveRet()
    {
        var move = MoveRetical.ReadValue<Vector3>();

        Movement.x = move.x;
        Movement.y = move.y;

        transform.Translate(Movement * RetSpeed * Time.deltaTime);
    }

    private void LookAhead()
    {
        Vector3 dir = Vector3.forward;

        if(Physics.Raycast(transform.position, dir, out RaycastHit hit))
        {
            //Debug.DrawRay(transform.position, dir, Color.yellow);
            

            if(hit.collider)
            {
                AimPosition = hit.collider.gameObject.transform;
            }
        }
    }
}
