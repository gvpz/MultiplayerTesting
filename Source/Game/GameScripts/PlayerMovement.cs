using System.Numerics;
using FlaxEngine;
using Vector3 = FlaxEngine.Vector3;

namespace Game;

/// <summary>
/// PlayerMovement Script.
/// </summary>
public class PlayerMovement : Script
{
    private RigidBody rb;
    private Vector3 groundCheck;

    private float moveSpeed = 50;
    private float speedMultiplier = 100;
    private float maxSpeed = 10;
    private float jumpForce = 750;
    private Float2 moveDirection;

    private bool IsGrounded() => Physics.RayCast(groundCheck, Vector3.Down, 15f, layerMask: 1 << 3);
    
    
    public override void OnStart()
    {
        rb = Actor.Scene.GetChild<RigidBody>();
        
    }

    public override void OnEnable()
    {
        PlayerInput.Instance.JumpPressed += Jump;
    }
    
    public override void OnDisable()
    {
        PlayerInput.Instance.JumpPressed -= Jump;
    }

    public override void OnUpdate()
    {
        groundCheck = Actor.GetChild("GroundCheck").Position;
        
        Debug.Log($"GroundCheck Position: {groundCheck}");
        Debug.Log($"IsGrounded: {IsGrounded()}");
        
        Move();
        DetermineGravity();
        
    }

    private void Move()
    {
        moveDirection = PlayerInput.Instance.MovementDirection;
        var trueMoveDir = new Vector3(moveDirection.X, 0, moveDirection.Y);
        trueMoveDir = trueMoveDir.Normalized;

        rb.MaxAngularVelocity = maxSpeed;
        rb.AddRelativeForce(trueMoveDir * moveSpeed * speedMultiplier * Time.DeltaTime, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        Debug.Log("Space pressed");
        if (!IsGrounded()) return;
        Debug.Log("Jump");
        rb.AddForce(Vector3.Up * jumpForce, ForceMode.VelocityChange);
    }

    private void DetermineGravity()
    {
        if (Physics.RayCast(groundCheck, Vector3.Down, out var rayCastHit, layerMask: 1 << 3))
            if(rayCastHit.Distance < 70) return;
        rb.AddForce(Vector3.Down * 9.8f, ForceMode.VelocityChange);
    }
    
    /*
     *  WHAT IS NEEDED FOR NETWORKING
     *
     *  
     *  packet
     *  all packages needed for sending (pos, input, 
     *  
     * 
     * 
     */
}
