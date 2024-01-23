using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// PlayerTransformPacket Script.
/// </summary>
public class PlayerTransformPacket : Packet
{
    public Vector3 position;
    public Quaternion rotation;
    
    public override void Serialize(ref NetworkMessage message)
    {
        message.WriteVector3(position);
        message.WriteQuaternion(rotation);
    }

    public override void Deserialize(ref NetworkMessage message)
    {
        position = message.ReadVector3();
        rotation = message.ReadQuaternion();
    }

    public override void ServerHandler(ref NetworkConnection connection)
    {
        var guid = NetworkManager.Instance.GuidByConnection(ref connection);
        var player = GameSession.Instance.GetPlayer(guid);
        player.Position = position;
        player.Rotation = rotation;
    }
}