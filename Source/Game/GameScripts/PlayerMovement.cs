using FlaxEngine;

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

    private bool IsGrounded() => Physics.CheckSphere(groundCheck, 0.2f, layerMask: 1 << 3);
    
    
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
        Move();
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
        if (!IsGrounded()) return;
        rb.AddForce(Vector3.Up * jumpForce, ForceMode.VelocityChange);
    }
    
    /*
     *  WHAT IS NEEDED FOR NETWORKING
     *
     *  
     *  packetRegistry
     *  all packages needed for sending (pos, input, 
     *  
     * 
     * 
     */
}
