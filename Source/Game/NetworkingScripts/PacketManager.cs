using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// PacketManager Script.
/// </summary>
public class PacketManager
{
    private Dictionary<int, Type> packets = new Dictionary<int, Type>();

    public void Register<T>() where T : Packet
    {
        packets.Remove(typeof(T).Name.DeterministicHash());
        packets.Add(typeof(T).Name.DeterministicHash(), typeof(T));
    }
    
    //Handles packet when network event is received
    public void Receive(ref NetworkEvent eventData, bool isServer = false)
    {
        try
        {
            var name = eventData.Message.ReadString();
            Debug.Log("Packet Name = " + name);
            var t = eventData.Message.ReadInt32();

            if (!packets.ContainsKey(t))
            {
                Debug.Log("Packet is not registered: t = " + t);
                return;
            }

            //Creates instance of packet
            var type = packets[t];
            var packet = (Packet)Activator.CreateInstance(type);
            packet.Sender = eventData.Sender;

            packet.Deserialize(ref eventData.Message);

            //Calls appropriate handler
            if (isServer)
                packet.ServerHandler(ref eventData.Sender);
            else
                packet.ClientHandler();
        }
        catch (Exception e)
        {
            Debug.Log($"Caught in Receive: {e}");
        }
    }

    //Hashes and serializes packet with referenced network message
    public void Send(Packet packet, ref NetworkMessage message)
    {
        message.WriteString(packet.GetType().Name);
        message.WriteInt32(packet.GetType().Name.DeterministicHash());
        packet.Serialize(ref message);
    }
}