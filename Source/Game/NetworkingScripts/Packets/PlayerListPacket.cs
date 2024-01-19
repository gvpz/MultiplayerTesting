using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// PlayerListPacket Script.
/// </summary>
public class PlayerListPacket : Packet
{
    public List<Player> playerList = new List<Player>();
    
    public override void Serialize(ref NetworkMessage message)
    {
        int length = playerList.Count + 1;
        message.WriteInt32(length);
        foreach (Player player in playerList)
        {
            message.WriteString(player.Name);
            message.WriteGuid(player.ID);
        }
        
        message.WriteString(GameSession.Instance.localPlayer.Name);
        message.WriteGuid(GameSession.Instance.localPlayer.ID);
    }

    public override void Deserialize(ref NetworkMessage message)
    {
        playerList.Clear();
        int length = message.ReadInt32();

        for(int i = 0; i < length; i++)
        {
            var guid = message.ReadGuid();
            var username = message.ReadString();
            Player player = new Player() {ID = guid, Name = username};
            playerList.Add(player);
        }
    }

    public override void ClientHandler()
    {
        foreach (var player in playerList)
        {
            if (!GameSession.Instance.playerList.Contains(player))
            {
                GameSession.Instance.playerList.Add(player);
            }
        }
    }
}
