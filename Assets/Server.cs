using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;
using Unity.Collections;

public class Server : ServerBase
{
    public List<NetReader> Readers;
    public List<NetSender> Senders;

    public GameObject Player;

    protected override void NewConnection(NetworkConnection connection)
    {
        // Making the ghost on the server side, basically it read position from the new connection
        var player = Instantiate(Player);
        var positionReader = player.AddComponent<PositionNetReader>();
        positionReader.ConnectionId = connection.InternalId;
        positionReader.Id = 4;
        positionReader.Target = player.transform;
        Readers.Add(positionReader);

        // Sending the ghost on the client side, basically it send the new ghost position outwardly to all clients
        var positionSender = player.AddComponent<PositionNetSender>();
        positionSender.ConnectionId = connection.InternalId;
        positionSender.Id = 4;
        Senders.Add(positionSender);

        var newConnectionSenderId = 99;
        var addPreviousGhostSenderId = 100;

        // Send a NewConnectionSender
        foreach (var c in m_Connections)
        {
            if (c.InternalId == connection.InternalId)
                continue;

            var writer = new DataStreamWriter(1000000, Allocator.Temp);
            writer.Write(newConnectionSenderId);
            writer.Write(connection.InternalId);
            m_Driver.Send(NetworkPipeline.Null, c, writer);
            writer.Dispose();
        }

        // New Connection need to also know about all the ghosts of the current connections
        var previousGhostWriter = new DataStreamWriter(1000000, Allocator.Temp);
        previousGhostWriter.Write(addPreviousGhostSenderId);

        var str = "";
        var count = 0;
        // Tell client to add ghost readers for each ids
        foreach (var c in m_Connections)
        {
            count++;

            if (c.InternalId == connection.InternalId)
                continue;

            str += $"{c.InternalId}";

            if (count < m_Connections.Length - 1)
            {
                str += ",";
            }
        }

        previousGhostWriter.WriteString(str);

        m_Driver.Send(NetworkPipeline.Null, connection, previousGhostWriter);
        previousGhostWriter.Dispose();
    }

    protected override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context c)
    {
        foreach (var reader in Readers)
        {
            if (reader.ConnectionId != null && reader.ConnectionId != connectionId)
                continue;

            var context = default(DataStreamReader.Context);
            var id = stream.ReadInt(ref context);

            if (id == reader.Id)
            {
                reader.Read(connectionId, stream, ref context);
            }
        }
    }

    protected override void Send(ref UdpNetworkDriver m_Driver, NetworkConnection networkConnection)
    {
        foreach (var sender in Senders)
        {
            var writer = sender.GetNew();

            writer.Write(sender.Id);
            sender.Write(writer);

            m_Driver.Send(NetworkPipeline.Null, networkConnection, writer);
            writer.Dispose();
        }
    }
}
