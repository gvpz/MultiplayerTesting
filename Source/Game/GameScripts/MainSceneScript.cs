using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// MainSceneScript Script.
/// </summary>
public class MainSceneScript : Script
{
    public Prefab playerPrefab;
    
    
    public override void OnStart()
    {
        
    }
    
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
        player.Actor = PrefabManager.SpawnPrefab(playerPrefab);;
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
        
    }
}
