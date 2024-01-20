using System;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// ConnectionRequestPacket Script.
/// </summary>
public class ConnectionRequestPacket : Packet
{
    public string Username = string.Empty;
    
    public override void Serialize(ref NetworkMessage message)
    {
        message.WriteString(Username);
    }

    public override void Deserialize(ref NetworkMessage message)
    {
        Username = message.ReadString();
    }

    public override void ServerHandler(ref NetworkConnection connection)
    {
        Debug.Log("Received Connection Request");
        //Sends a connection response.  Logic for acceptance required
        ConnectionResponsePacket responsePacket = new ConnectionResponsePacket();
        responsePacket.ID = NetworkManager.Instance.GuidByConnection(ref connection);
        responsePacket.SceneID = Level.GetScene(0).ID;
        responsePacket.State = ConnectionResponsePacket.ConnectionState.Accepted;
        NetworkManager.Instance.Send(responsePacket, NetworkChannelType.ReliableOrdered, ref connection);

        //GameSession.Instance.GetPlayer(responsePacket.ID);
        
        
        //Send all initial packets needed
    }
}