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
        Debug.Log("Received Connection Response");
        if (State == ConnectionState.Accepted)
        {
            Debug.Log("Server accepted connection");
            GameSession.Instance.localPlayer.ID = ID;
            SceneManager.Instance.LoadScene(SceneID, Level.GetScene(0));
        }
        else
        {
            NetworkManager.Instance.Disconnect();
            Debug.Log("Connection Rejected");
        }
    }
}