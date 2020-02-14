using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.UI;
using System.Collections.Generic;

public class Server : ServerBase
{
    public List<NetReader> Readers;
    public List<NetSender> Senders;

    protected override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        foreach (var reader in Readers)
        {
            reader.Read(connectionId, stream, ref context);
        }
    }

    protected override void Send(ref UdpNetworkDriver m_Driver, NetworkConnection networkConnection)
    {
        foreach (var sender in Senders)
        {
            var writer = sender.Write();
            m_Driver.Send(NetworkPipeline.Null, networkConnection, writer);
            writer.Dispose();
        }
    }
}
