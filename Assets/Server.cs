using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class Server : ServerBase
{
    public List<NetReader> Readers;
    public List<NetSender> Senders;

    public GameObject Player;

    protected override void NewConnection(NetworkConnection connection)
    {
        // Making the ghost
        var player = Instantiate(Player);
        var positionReader = player.AddComponent<PositionNetReader>();
        positionReader.Id = 4;
        positionReader.Target = player.transform;
        Readers.Add(positionReader);
    }

    protected override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context c)
    {
        foreach (var reader in Readers)
        {
            var context = default(DataStreamReader.Context);
            var id = stream.ReadInt(ref context);

            if(id == reader.Id)
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
