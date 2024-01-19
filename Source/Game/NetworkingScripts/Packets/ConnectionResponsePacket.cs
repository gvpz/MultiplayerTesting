using System;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// ConnectionResponsePacket Script.
/// </summary>
public class ConnectionResponsePacket : Packet
{
    public enum ConnectionState : byte
    {
        Accepted,
        Rejected
    }

    public ConnectionState State;
    public Guid ID = Guid.Empty;
    public Guid SceneID = Guid.Empty;
    //Need to change SceneID to int

    public override void Serialize(ref NetworkMessage message)
    {
        message.WriteByte((byte)State);
        var bytes = ID.ToByteArray();
        message.WriteInt32(bytes.Length);
        message.WriteBytes(bytes, bytes.Length);

        bytes = SceneID.ToByteArray();
        message.WriteInt32(bytes.Length);
        message.WriteBytes(bytes, bytes.Length);
    }

    public override void Deserialize(ref NetworkMessage message)
    {
        State = (ConnectionState)message.ReadByte();
        var length = message.ReadInt32();
        byte[] bytes = new byte[length];
        message.ReadBytes(bytes, length);
        ID = new Guid(bytes);
        
        length = message.ReadInt32();
        bytes = new byte[length];
        message.ReadBytes(bytes, length);
        SceneID = new Guid(bytes);
    }

    public override void ClientHandler()
    {
        if (State == ConnectionState.Accepted)
        {
            GameSession.Instance.localPlayer.ID = ID;
            Level.LoadSceneAsync(SceneID);
        }
        else
        {
            Debug.Log("Connection Rejected");
        }
    }
}