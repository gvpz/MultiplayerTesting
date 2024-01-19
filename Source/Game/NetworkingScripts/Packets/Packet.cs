using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// Packet Script.
/// </summary>
public abstract class Packet
{
    public NetworkConnection Sender;

    public abstract void Serialize(ref NetworkMessage message);

    public abstract void Deserialize(ref NetworkMessage message);

    public virtual void ServerHandler(ref NetworkConnection connection)
    {
    }

    public virtual void ClientHandler()
    {
    }
}
