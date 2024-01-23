using FlaxEngine;
using FlaxEngine.Networking;
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
    private float movementDrag = 5;
    private float stoppingDrag = 10;
    private Float2 moveDirection;

    private float lastPacketSent;

    private bool IsGrounded() => Physics.RayCast(groundCheck, Vector3.Down, 15f, layerMask: 1 << 3);
    
    
    public override void OnStart()
    {
        rb = Actor.Scene.GetChild<RigidBody>();
        rb.MaxAngularVelocity = maxSpeed;

        GameSession.Instance.localPlayer.Actor = Actor;
        GameSession.Instance.localPlayer.Position = Actor.Position;
        GameSession.Instance.localPlayer.ID = Actor.ID;
        GameSession.Instance.localPlayer.Name = GameSession.Instance.localPlayer.Name;
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
        DetermineGravity();

        if (Time.UnscaledGameTime - lastPacketSent >= 0.01f)
        {
            SendTransformPacket();
        }
    }

    private void Move()
    {
        moveDirection = PlayerInput.Instance.MovementDirection;
        var trueMoveDir = new Vector3(moveDirection.X, 0, moveDirection.Y);
        trueMoveDir = trueMoveDir.Normalized;
        
        rb.AddRelativeForce(trueMoveDir * moveSpeed * speedMultiplier * Time.DeltaTime, ForceMode.VelocityChange);

        rb.LinearDamping = moveDirection == Float2.Zero ? stoppingDrag : movementDrag;
        GameSession.Instance.localPlayer.Position = Actor.Position;
    }

    private void Jump()
    {
        if (!IsGrounded()) return;
        rb.AddForce(Vector3.Up * jumpForce, ForceMode.VelocityChange);
    }

    private void DetermineGravity()
    {
        groundCheck = rb.GetChild("GroundCheck").Position;

        if (Physics.RayCast(groundCheck, Vector3.Down, out var rayCastHit, layerMask: 1 << 3))
            if(rayCastHit.Distance < 70) return;
        rb.AddForce(Vector3.Down * 9.8f, ForceMode.VelocityChange);
    }

    private void SendTransformPacket()
    {
        var packet = new PlayerTransformPacket
        {
            position = rb.Transform.Translation,
            rotation = rb.Transform.Orientation
        };
        NetworkManager.Instance.Send(packet, NetworkChannelType.UnreliableOrdered);
        lastPacketSent = Time.UnscaledGameTime;
    }
}
