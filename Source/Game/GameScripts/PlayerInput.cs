using System;
using FlaxEngine;

namespace Game;

/// <summary>
/// PlayerInput Script.
/// </summary>
public class PlayerInput : Script
{
    
    public static Action JumpPressed;
    public static Action Fire;
    public static Action SecondaryFire;


    public static Float2 MovementDirection;

    public static float MouseX;
    public static float MouseY;

    public override void OnUpdate()
    {

        MovementDirection.X = Input.GetAxisRaw("Horizontal");
        MovementDirection.Y = Input.GetAxisRaw("Vertical");

        MouseX = Input.GetAxisRaw("Mouse X");
        MouseY = Input.GetAxisRaw("Mouse Y");
        
        if (Input.GetAction("Jump"))
        {
            JumpPressed?.Invoke();
        }

        if (Input.GetAction("Fire"))
        {
            Fire?.Invoke();
        }

        if (Input.GetAction("SecondaryFire"))
        {
            SecondaryFire?.Invoke();
        }
    }
    
}
