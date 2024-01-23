using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// AllPlayerTransformsPacket Script.
/// </summary>
public class AllPlayerTransformsPacket : Packet
{
    public List<TransformEntry> Transforms = new List<TransformEntry>();

    public struct TransformEntry
    {
        public Guid Guid;
        public Vector3 Position;
        public Quaternion Rotation;
    }
    
    public override void Serialize(ref NetworkMessage message)
    {
        message.WriteInt32(Transforms.Count);
        foreach (var transform in Transforms)
        {
            message.WriteGuid(transform.Guid);
            message.WriteVector3(transform.Position);
            message.WriteQuaternion(transform.Rotation);
        }
    }

    public override void Deserialize(ref NetworkMessage message)
    {
        var count = message.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var transform = new TransformEntry
            {
                Guid = message.ReadGuid(),
                Position = message.ReadVector3(),
                Rotation = message.ReadQuaternion()
            };
            Transforms.Add(transform);
        }
    }

    public override void ClientHandler()
    {
        foreach (var transform in Transforms)
        {
            if (transform.Guid == GameSession.Instance.localPlayer.ID) continue;
            var player = GameSession.Instance.GetPlayer(transform.Guid);
            player.Position = transform.Position;
            player.Rotation = transform.Rotation;
        }
    }
}