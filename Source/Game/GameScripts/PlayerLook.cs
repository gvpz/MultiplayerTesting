using FlaxEngine;

namespace Game;

/// <summary>
/// PlayerLook Script.
/// </summary>
public class PlayerLook : Script
{
    private Transform cam;
    private Transform player;
    
    private float mouseX;
    private float mouseY;

    private float xRotation;
    private float yRotation;
    
    private float sensitivity = 100;
    private float rotationMultiplier = 1;
    
    public override void OnStart()
    {
        Screen.CursorLock = CursorLockMode.Locked;
        Screen.CursorVisible = false;
        
        cam = Actor.Transform;
        player = Actor.Parent.Transform;
    }

    public override void OnFixedUpdate()
    {
        Look();
    }

    private void Look()
    {
        mouseX = PlayerInput.MouseX;
        mouseY = PlayerInput.MouseY;

        xRotation -= mouseY * sensitivity * rotationMultiplier;
        yRotation += mouseX * sensitivity * rotationMultiplier;
        
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        cam.Orientation = Quaternion.Euler(xRotation, 0, 0);
        player.Orientation = Quaternion.Euler(0, yRotation, 0);
    }
}
