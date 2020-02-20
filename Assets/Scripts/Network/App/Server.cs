using Unity.Networking.Transport;
using System.Collections.Generic;
using Unity.Collections;
using System;
using System.Linq;

public class Server : ServerBase
{
    public List<NetReader> Readers;
    public List<NetSender> Senders;

    public Action<NetworkConnection> OnClientConnected;
    public Action<int> OnClientDisconnected;

    public UdpNetworkDriver Driver => m_Driver;
    public NativeList<NetworkConnection> Connections => m_Connections;

    protected override void ClientConnected(NetworkConnection connection)
    {
        OnClientConnected?.Invoke(connection);
    }

    public void Disconnect(int internalId)
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (m_Connections[i].InternalId == internalId)
            {
                m_Driver.Disconnect(m_Connections[i]);
                ClearReferences(internalId);
                break;
            }
        }

        OnClientDisconnected?.Invoke(internalId);
    }

    public void AddReader(NetReader reader)
    {
        Readers.Add(reader);
    }

    public void AddSender(NetSender sender)
    {
        Senders.Add(sender);
    }

    protected override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context c)
    {
        foreach (var reader in Readers)
        {
            var context = default(DataStreamReader.Context);

            if (reader.ConnectionId != null && reader.ConnectionId != connectionId)
                continue;

            var id = stream.ReadInt(ref context);

            if (reader.InstanceId != null)
            {
                var instanceId = stream.ReadInt(ref context);
                if (instanceId != reader.InstanceId)
                    continue;
            }

            if (id == reader.Id)
                reader.Read(connectionId, stream, ref context);
        }
    }

    protected override void Write(ref UdpNetworkDriver m_Driver, NetworkConnection networkConnection)
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

    public void Write(DataStreamWriter writer, NetworkConnection connection)
    {
        m_Driver.Send(NetworkPipeline.Null, connection, writer);
        writer.Dispose();
    }

    public void WriteToAllConnections(DataStreamWriter writer) 
    {
        foreach (var connection in m_Connections) 
        {
            m_Driver.Send(NetworkPipeline.Null, connection, writer);
            writer.Dispose();
        }
    }

    public void ClearReferences(int instanceId) 
    {
        for (int i = 0; i < Senders.Count; i++)
        {
            if (Senders[i].InstanceId == instanceId)
            {
                Senders.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < Readers.Count; i++)
        {
            if (Readers[i].InstanceId == instanceId)
            {
                Readers.RemoveAt(i);
                i--;
            }
        }
    }
}
