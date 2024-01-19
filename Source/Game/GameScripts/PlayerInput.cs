using System;
using FlaxEngine;

namespace Game;

/// <summary>
/// PlayerInput Script.
/// </summary>
public class PlayerInput : GamePlugin
{
    
    public Action JumpPressed;
    public Action Fire;
    public Action SecondaryFire;


    public Float2 MovementDirection;

    public float MouseX;
    public float MouseY;

    public override void Initialize()
    {
        Scripting.Update += OnUpdate;
    }

    public override void Deinitialize()
    {
        Scripting.Update -= OnUpdate;
    }

    public void OnUpdate()
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

    private static PlayerInput instance;

    public static PlayerInput Instance
    {
        get
        {
            if (instance == null)
                instance = PluginManager.GetPlugin<PlayerInput>();
            return instance;
        }
    }
}