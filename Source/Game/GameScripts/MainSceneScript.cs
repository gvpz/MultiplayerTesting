using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// MainSceneScript Script.
/// </summary>
public class MainSceneScript : Script
{
    public Prefab networkPlayerPrefab;
    private float lastTransformPacketSent;
    
    
    public override void OnEnable()
    {
        GameSession.Instance.OnPlayerAdded += OnPlayerAdded;
        GameSession.Instance.OnPlayerRemoved += OnPlayerRemoved;
    }

    public override void OnDisable()
    {
        GameSession.Instance.OnPlayerAdded -= OnPlayerAdded;
        GameSession.Instance.OnPlayerRemoved -= OnPlayerRemoved;
    }

    public void OnPlayerAdded(Player player)
    {
        player.Actor = PrefabManager.SpawnPrefab(networkPlayerPrefab, Actor);
        var playerScript = player.Actor.GetScript<NetworkPlayer>();
        playerScript.player = player;
        player.Actor.Name = "Player_" + player.Name;
    }

    public void OnPlayerRemoved(Player player)
    {
        Destroy(player.Actor);
    }

    public override void OnUpdate()
    {
        if (!NetworkManager.Instance.IsServer || !(Time.UnscaledGameTime - lastTransformPacketSent >= 0.01f)) return;
        var transformEntry = new AllPlayerTransformsPacket.TransformEntry();
        var transformPacket = new AllPlayerTransformsPacket();
        foreach (var player in GameSession.Instance.playerList)
        {
            transformEntry.Guid = player.ID;
            transformEntry.Position = player.Position;
            transformEntry.Rotation = player.Rotation;
            transformPacket.Transforms.Add(transformEntry);
        }

        transformEntry.Guid = GameSession.Instance.localPlayer.ID;
        transformEntry.Position = GameSession.Instance.localPlayer.Position;
        transformEntry.Rotation = GameSession.Instance.localPlayer.Rotation;
        transformPacket.Transforms.Add(transformEntry);

        NetworkManager.Instance.SendAll(transformPacket, NetworkChannelType.UnreliableOrdered);
        lastTransformPacketSent = Time.UnscaledGameTime;
    }
}