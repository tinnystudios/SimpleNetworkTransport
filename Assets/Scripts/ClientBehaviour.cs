using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class ClientBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool m_Done;
    private bool m_Connected;

    public List<NetSender> Senders;
    public List<NetReader> Readers;

    void Start ()
    {
        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        Connect();
    }

    public void Connect()
    {
        m_Connection = default(NetworkConnection);

        //var endpoint = NetworkEndPoint.LoopbackIpv4;
        //endpoint.Port = 9000;

        var endpoint = NetworkEndPoint.Parse("192.168.1.82", 9000);
        m_Connection = m_Driver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    public void Send(DataStreamWriter writer)
    {
        m_Connection.Send(m_Driver, writer);
        writer.Dispose();
    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!m_Done)
                Debug.Log("Something went wrong during connect");

            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");
                m_Connected = true;
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                // Temp
                var readers = new List<NetReader>(Readers);
                foreach (var reader in readers)
                {
                    var readerCtx = default(DataStreamReader.Context);

                    var id = stream.ReadInt(ref readerCtx);
                    if (id == reader.Id)
                    {
                        if (reader.ConnectionId != null)
                        {
                            var targetConnectionId = stream.ReadInt(ref readerCtx);
                            if (targetConnectionId != reader.ConnectionId)
                                continue;
                        }

                        reader.Read(0, stream, ref readerCtx);
                    }
                }

                // At the moment this is to keep the client connected.
                foreach (var sender in Senders)
                {
                    var writer = sender.GetNew();
                    writer.Write(sender.Id);
                    sender.Write(writer);
                    Send(writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                Connect();
            }
        }
    }
}