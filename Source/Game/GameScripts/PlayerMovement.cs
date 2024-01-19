using FlaxEngine;

namespace Game;

/// <summary>
/// PlayerMovement Script.
/// </summary>
public class PlayerMovement : Script
{
    private RigidBody rb;

    private float moveSpeed = 50;
    private float speedMultiplier = 1;
    private float maxSpeed = 10;
    private Vector2 moveDirection;
    
    
    public override void OnStart()
    {
        rb = Actor.Scene.GetChild<RigidBody>();
    }

    public override void OnEnable()
    {
        PlayerInput.JumpPressed += Jump;
    }
    
    public override void OnDisable()
    {
        PlayerInput.JumpPressed -= Jump;
    }

    public override void OnFixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        moveDirection = PlayerInput.MovementDirection;
        var trueMoveDir = new Vector3(moveDirection.X, 0, moveDirection.Y);

        rb.MaxAngularVelocity = maxSpeed;
        rb.AddRelativeForce(trueMoveDir * moveSpeed * speedMultiplier, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        Debug.Log("JUMP");
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
