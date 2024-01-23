using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;

/// <summary>
/// NetworkPlayer Script.
/// </summary>
public class NetworkPlayer : Script
{
    public Player player;

    public override void OnUpdate()
    {
        var transform = Actor.Transform;
        transform.Translation = Vector3.Lerp(transform.Translation, player.Position, 0.04f);
        transform.Orientation = Quaternion.Lerp(transform.Orientation, player.Rotation, 0.04f);
        Actor.Transform = transform;

        var label = Actor.FindActor<UIControl>();
        label.Get<Label>().Text = player.Name;
        Actor.Name = "Player_" + player.Name;
    }
}
