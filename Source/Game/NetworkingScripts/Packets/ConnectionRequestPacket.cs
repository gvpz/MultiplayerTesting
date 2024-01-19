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
        //Sends a connection response
        ConnectionResponsePacket responsePacket = new ConnectionResponsePacket();
        responsePacket.ID = NetworkManager.Instance.GuidByConnection(ref connection);
        responsePacket.SceneID = Level.GetScene(0).ID;
        
        //Logic for acceptance required
        responsePacket.State = ConnectionResponsePacket.ConnectionState.Accepted;
        GameSession.Instance.GetPlayer(responsePacket.ID).Name = Username;
        
        NetworkManager.Instance.Send(responsePacket, NetworkChannelType.ReliableOrdered, ref connection);
        
        //Send all initial packets needed
    }
}
