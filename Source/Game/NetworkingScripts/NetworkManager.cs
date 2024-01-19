using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Networking;
using Debug = FlaxEngine.Debug;

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
        packetManager = new PacketManager();
        connectionManager = new ConnectionManager();

        //Register Packets
        packetManager.Register<ConnectionRequestPacket>();
        packetManager.Register<ConnectionResponsePacket>();

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
            Debug.Log(peer.PopEvent(out var eventRef));
            Debug.Log(eventRef);
            while (peer.PopEvent(out var eventData))
            {
                Debug.Log("Server: " + eventData);
                if (eventData.EventType == NetworkEventType.Connected)
                {
                    connectionManager.Add(ref eventData.Sender, GameSession.Instance.AddPlayer());
                    Debug.Log(eventData.Sender + " has connected!");
                }
                else if (eventData.EventType == NetworkEventType.Disconnected || 
                         eventData.EventType == NetworkEventType.Timeout)
                {
                    var guid = GuidByConnection(ref eventData.Sender);
                    connectionManager.Remove(ref guid);
                    GameSession.Instance.RemovePlayer(ref guid);
                    Debug.Log(eventData.Sender + " has disconnected!");
                }
                else if (eventData.EventType == NetworkEventType.Message)
                {
                    packetManager.Receive(ref eventData, isServer);
                    peer.RecycleMessage(eventData.Message);
                }
            }
        }
        //If client is not server
        else
        {
            while (peer.PopEvent(out var eventData))
            {
                Debug.Log("Client: " + eventData);
                if (eventData.EventType == NetworkEventType.Message)
                {
                    packetManager.Receive(ref eventData, isServer);
                    peer.RecycleMessage(eventData.Message);
                    Debug.Log("Message Received");
                }
                else if (eventData.EventType == NetworkEventType.Connected)
                {
                    Send(new ConnectionRequestPacket() { Username = GameSession.Instance.localPlayer.Name },
                        NetworkChannelType.ReliableOrdered);
                    Debug.Log("Sent connection request packet");
                }
                else if (eventData.EventType == NetworkEventType.Disconnected ||
                         eventData.EventType == NetworkEventType.Timeout)
                {
                    Disconnect();
                    NetworkPeer.ShutdownPeer(peer);
                    Debug.Log("Timed out/Disconnected");
                }

            }
        }
    }

    //Makes player the host of the server through designated Port (port) with Username (username)
    public bool Host(string username, ushort port)
    {
        Debug.Log("Calling NetworkManager.Host()");
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
        Debug.Log("Successfully finished NetworkManager.Host()");
        return true;
    } 

    //Connects player to IP (address) and Port (port) with a Username (username)
    public bool Connect(string username, string address, ushort port)
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
    
    public Guid GuidByConnection(ref NetworkConnection connection)
    {
        return connectionManager.GuidByConnection(ref connection);
    }

    static async Task<string> GetIpAddress()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                string response = await httpClient.GetStringAsync("https://api64.ipify.org?format=json");
                // Parse the JSON response to get the public IP address
                // The response format typically includes the IP address in a property like "ip"
                // Example: {"ip":"123.456.789.012"}
                // You can use a JSON parsing library or simple string manipulation to extract the IP address.
                // For simplicity, let's assume a straightforward case:
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

    public async void GetPublicIPAddress()
    {
        ipAddress = await GetIpAddress();
        Debug.Log("IP: " + ipAddress);
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