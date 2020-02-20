using Unity.Networking.Transport;
using System.Collections.Generic;
using Unity.Collections;
using System;

public class Server : ServerBase
{
    public List<NetReader> Readers;
    public List<NetSender> Senders;

    public Action<NetworkConnection> OnClientConnected;
    public Action<NetworkConnection> OnClientDisconnected;

    public UdpNetworkDriver Driver => m_Driver;
    public NativeList<NetworkConnection> Connections => m_Connections;

    protected override void ClientConnected(NetworkConnection connection)
    {
        OnClientConnected?.Invoke(connection);
    }

    protected override void ClientDisconnected(NetworkConnection connection)
    {
        OnClientDisconnected?.Invoke(connection);
    }

    public void Disconnect(int internalId)
    {

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
        foreach (var s in Senders) 
            Senders.Remove(s);

        foreach (var r in Readers)
            Readers.Remove(r);
    }
}
