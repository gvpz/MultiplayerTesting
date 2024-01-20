using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// GameSession Script.
/// </summary>
public class GameSession : GamePlugin
{
    public delegate void OnPlayerAddedHandler(Player player);
    public event OnPlayerAddedHandler OnPlayerAdded;

    public delegate void OnPlayerRemovedHandler(Player player);
    public event OnPlayerRemovedHandler OnPlayerRemoved;

    public Prefab playerPrefab;
    
    public List<Player> playerList = new List<Player>();

    public Player localPlayer;
    
    public override void Initialize()
    {
        localPlayer = new Player();
        Scripting.Update += OnUpdate;
    }

    public override void Deinitialize()
    {
        base.Deinitialize();

        if (instance == this)
        {
            instance = null;
        }
    }

    public Player AddPlayer()
    {
        var player = new Player() { ID = Guid.NewGuid() };
        AddPlayer(player);
        return player;
    }

    public Player AddPlayer(ref Guid guid, string name)
    {
        var player = new Player() { ID = guid, Name = name };
        AddPlayer(player);
        return player;
    }

    public void AddPlayer(Player player)
    {
        playerList.Add(player);
        OnPlayerAdded?.Invoke(player);
    }

    public bool RemovePlayer(ref Guid guid)
    {
        for (var i = playerList.Count - 1; i >= 0; i--)
        {
            if (playerList[i].ID != guid) continue;
            var player = playerList[i];
            playerList.RemoveAt(i);
            OnPlayerRemoved?.Invoke(player);
            return true;
        }

        return false;
    }

    public Player GetPlayer(Guid guid)
    {
        if (localPlayer.ID == guid)
            return localPlayer;
        foreach (Player player in playerList)
        {
            if (player.ID == guid)
            {
                return player;
            }
        }

        return null;
    }

    private void SendPackets()
    {

    }

    private void OnUpdate()
    {
        if (Time.DeltaTime >= 0.016)
        {
            SendPackets();
        }
    }

    private static GameSession instance;

    public static GameSession Instance
    {
        get
        {
            if (instance == null)
                instance = PluginManager.GetPlugin<GameSession>();
            return instance;
        }    }
}
