using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// ConnectionManager Script.
/// </summary>
public class ConnectionManager
{
    private Dictionary<Guid, NetworkConnection> connectionsByGuid = new Dictionary<Guid, NetworkConnection>();
    private Dictionary<NetworkConnection, Guid> guidByConnections = new Dictionary<NetworkConnection, Guid>();

    public void Add(ref NetworkConnection connection, Player player)
    {
        connectionsByGuid.Add(player.ID, connection);
        guidByConnections.Add(connection, player.ID);
    }

    public void Remove(ref NetworkConnection connection)
    {
        var guid = guidByConnections[connection];
        connectionsByGuid.Remove(guid);
        guidByConnections.Remove(connection);
    }
    
    public void Remove(ref Guid guid)
    {
        var connection = connectionsByGuid[guid];
        connectionsByGuid.Remove(guid);
        guidByConnections.Remove(connection);
    }

    public void Clear()
    {
        connectionsByGuid.Clear();
        guidByConnections.Clear();
    }

    public Guid GuidByConnection(ref NetworkConnection connection)
    {
        return guidByConnections.ContainsKey(connection) ? guidByConnections[connection] : default;
    }

    public NetworkConnection ConnectionByGuid(ref Guid guid)
    {
        return connectionsByGuid.ContainsKey(guid) ? connectionsByGuid[guid] : default;
    }

    public NetworkConnection[] ToArray()
    {
        return connectionsByGuid.Values.ToArray();
    }
}
