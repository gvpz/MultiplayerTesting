using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// PlayerConnectedPacket Script.
/// </summary>
public class PlayerConnectedPacket : Packet
{
    public Guid ID;
    public string Username;
    
    public override void Serialize(ref NetworkMessage message)
    {
        message.WriteGuid(ID);
        message.WriteString(Username);
    }

    public override void Deserialize(ref NetworkMessage message)
    {
        ID = message.ReadGuid();
        Username = message.ReadString();
    }

    public override void ClientHandler()
    {
        if (ID == GameSession.Instance.localPlayer.ID) return;
        GameSession.Instance.AddPlayer(ref ID, Username);
    }
}
