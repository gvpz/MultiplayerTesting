using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// NetworkManager Script.
/// </summary>
public class NetworkManager : GamePlugin
{
    private NetworkPeer peer;
    private ConnectionManager connectionManager;
    private PacketManager packetManager;

    private bool isConnected;
    private bool isServer;

    public bool IsConnected => isConnected;
    public bool IsServer => isServer;

    public static string ipAddress = null;

    public override void Initialize()
    {
        base.Initialize();
        isConnected = false;
        packetManager = new PacketManager();

        //Register Packets
        packetManager.Register<AllPlayerTransformsPacket>();
        packetManager.Register<ConnectionRequestPacket>();
        packetManager.Register<ConnectionResponsePacket>();
        packetManager.Register<PlayerConnectedPacket>();
        packetManager.Register<PlayerListPacket>();
        packetManager.Register<PlayerTransformPacket>();

        connectionManager = new ConnectionManager();
        GetPublicIPAddress();
        Scripting.Update += OnUpdate;
    }

    public override void Deinitialize()
    {
        base.Deinitialize();
        Scripting.Update -= OnUpdate;
        isConnected = false;
        isServer = false;
        Disconnect();
        if (instance == this)
            instance = null;
    }

    public void OnUpdate()
    {
        if (!isConnected)
            return;

        //If client is Server
        if (isServer)
        {
            while (peer.PopEvent(out var eventData))
            {
                switch (eventData.EventType)
                {
                    //Server received connected event type
                    case NetworkEventType.Connected:
                    {
                        connectionManager.Add(ref eventData.Sender, GameSession.Instance.AddPlayer());
                        break;
                    }
                    //Server received disconnected or timeout event type
                    case NetworkEventType.Disconnected:
                    case NetworkEventType.Timeout:
                    {
                        packetManager.Receive(ref eventData, isServer);
                        var guid = GuidByConnection(ref eventData.Sender);
                        connectionManager.Remove(ref guid);
                        GameSession.Instance.RemovePlayer(ref guid);
                        break;
                    }
                    //Server received message event type
                    case NetworkEventType.Message:
                    {
                        packetManager.Receive(ref eventData, isServer);
                        peer.RecycleMessage(eventData.Message);
                        break;
                    }
                    default:
                        return;
                }
            }
        }
        //If client is not server
        else
        {
            while (peer.PopEvent(out var eventData))
            {
                switch (eventData.EventType)
                {
                    //Client received message event type
                    case NetworkEventType.Message:
                    {
                        packetManager.Receive(ref eventData, isServer);
                        peer.RecycleMessage(eventData.Message);
                        break;
                    }
                    //Client received connected event type
                    case NetworkEventType.Connected:
                    {
                        Send(new ConnectionRequestPacket { Username = GameSession.Instance.localPlayer.Name },
                            NetworkChannelType.ReliableOrdered);
                        break;
                    }
                    //Client received disconnected or timeout network type
                    case NetworkEventType.Disconnected:
                    case NetworkEventType.Timeout:
                    {
                        Disconnect();
                        NetworkPeer.ShutdownPeer(peer);
                        break;
                    }
                }
            }
        }
    }

    //Makes player the host of the server through designated Port (port) with Username (username)
    public bool Host(string username, ushort port)
    {
        try
        {
            peer = NetworkPeer.CreatePeer(new NetworkConfig
            {
                NetworkDriver = new ENetDriver(),
                ConnectionsLimit = 32,
                MessagePoolSize = 256,
                MessageSize = 1500,
                Address = "any",
                Port = port
            });
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        if (!peer.Listen()) return false;
        isConnected = true;
        isServer = true;
        return true;
    } 

    //Connects player to IP (address) and Port (port) with a Username (username)
    public bool Connect(string username, string address, ushort port)
    {
        if (peer == null)
        {
            peer = NetworkPeer.CreatePeer(new NetworkConfig
            {
                NetworkDriver = new ENetDriver(),
                ConnectionsLimit = 32,
                MessagePoolSize = 256,
                MessageSize = 1500,
                Address = address,
                Port = port,
            });
        }

        GameSession.Instance.localPlayer.Name = username;
        
        if(!isConnected)
            isConnected = true;
        return !peer.Connect();
    }

    //Disconnects player from session
    public void Disconnect()
    {
        NetworkPeer.ShutdownPeer(peer);
        isConnected = false;
    }
    
    //Send a packet via network type
    public void Send(Packet packet, NetworkChannelType type)
    {
        if (!isConnected)
            return;

        var message = peer.BeginSendMessage();
        packetManager.Send(packet, ref message);
        
        if (isServer)
            peer.EndSendMessage(type, message, connectionManager.ToArray());
        else
            peer.EndSendMessage(type, message);
    }

    //Send a packet via network type (server)
    public void Send(Packet packet, NetworkChannelType type, ref NetworkConnection connection)
    {
        if (!isConnected || !isServer)
            return;

        var message = peer.BeginSendMessage();
        packetManager.Send(packet, ref message);
        peer.EndSendMessage(type, message, connection);
    }
    
    //Send packet via network type to all clients
    public void SendAll(Packet packet, NetworkChannelType type)
    {
        if (!isServer || !isConnected)
            return;
        var msg = peer.BeginSendMessage();
        packetManager.Send(packet, ref msg);
        peer.EndSendMessage(type, msg, connectionManager.ToArray());
    }
    
    //Returns Guid with reference to a network connection
    public Guid GuidByConnection(ref NetworkConnection connection)
    {
        return connectionManager.GuidByConnection(ref connection);
    }

    //Gets IP address via HTTPClient
    static async Task<string> GetIpAddress()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                string response = await httpClient.GetStringAsync("https://api64.ipify.org?format=json");
                int startIndex = response.IndexOf("\"ip\":\"") + 6;
                int endIndex = response.IndexOf("\"", startIndex);
                return response.Substring(startIndex, endIndex - startIndex);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., network error)
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }

    //Calls GetIP task
    public async void GetPublicIPAddress()
    {
        ipAddress = await GetIpAddress();
    }
    
    private static NetworkManager instance;

    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
                instance = PluginManager.GetPlugin<NetworkManager>();
            return instance;
        }
    }
}