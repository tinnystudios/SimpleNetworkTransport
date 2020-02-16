using Unity.Networking.Transport;
using System.Collections.Generic;
using Unity.Collections;
using System;

public class Server : ServerBase
{
    public List<NetReader> Readers;
    public List<NetSender> Senders;

    public Action<NetworkConnection> OnNewConnection;

    public UdpNetworkDriver Driver => m_Driver;
    public NativeList<NetworkConnection> Connections => m_Connections;

    protected override void NewConnection(NetworkConnection connection)
    {
        OnNewConnection?.Invoke(connection);
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
            if (reader.ConnectionId != null && reader.ConnectionId != connectionId)
                continue;

            var context = default(DataStreamReader.Context);
            var id = stream.ReadInt(ref context);

            if (id == reader.Id)
                reader.Read(connectionId, stream, ref context);
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
