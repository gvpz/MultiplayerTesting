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
        //Sends a connection response.  Logic for acceptance required
        var responsePacket = new ConnectionResponsePacket
        {
            ID = NetworkManager.Instance.GuidByConnection(ref connection),
            SceneID = Level.GetScene(0).ID,
            State = ConnectionResponsePacket.ConnectionState.Accepted
        };
        NetworkManager.Instance.Send(responsePacket, NetworkChannelType.ReliableOrdered, ref connection);
        GameSession.Instance.GetPlayer(responsePacket.ID).Name = Username;
        Debug.Log("Username = " + Username);

        var listPacket = new PlayerListPacket
        {
            playerList = GameSession.Instance.playerList
        };
        NetworkManager.Instance.Send(listPacket, NetworkChannelType.Reliable);

        var playerConnectedPacket = new PlayerConnectedPacket()
        {
            ID = responsePacket.ID,
            Username = Username
        };
        NetworkManager.Instance.SendAll(playerConnectedPacket, NetworkChannelType.Reliable);
    }
}