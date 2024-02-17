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
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveRet();
    }

    private void MoveRet()
    {
        var move = MoveRetical.ReadValue<Vector3>();

        Movement.x = move.x;
        Movement.y = move.y;

        transform.Translate(Movement * RetSpeed * Time.deltaTime);
    }
}
