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
            Debug.Log($"Message Length before ReadInt32: {eventData.Message.Length}");
            Debug.Log($"Message Read Position before ReadInt32: {eventData.Message.Position}");
            var t = eventData.Message.ReadInt32();
            Debug.Log($"Received hash: {t}");
            Debug.Log($"Message Read Position after ReadInt32: {eventData.Message.Position}");


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
        Debug.Log("From Send Before Hash: " + packet + " " + message.Length);
        message.WriteInt32(packet.GetType().Name.DeterministicHash());
        Debug.Log("From Send After Hash Assignment: " + packet + " " + message.Length);
        packet.Serialize(ref message);
        Debug.Log("From Send After Serialize: " + packet + " " + message.Length);
        Debug.Log($"Hash for {packet} = {packet.GetType().Name.DeterministicHash()}");
    }
}