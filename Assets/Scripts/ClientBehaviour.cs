using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;
using System.Collections;

public class ClientBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool m_Done;
    private bool m_Connected;

    public List<NetSender> Senders;
    public List<NetReader> Readers;

    public NetworkConfig NetworkConfig;

    private float _frame;

    void Start ()
    {
        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        Connect();

        StartCoroutine(Run());
    }

    public void Connect()
    {
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkConfig.GetClientEndPoint();
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

    private IEnumerator Run()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5F);

            // At the moment this is to keep the client connected.
            foreach (var sender in Senders)
            {
                var writer = sender.GetNew();
                writer.Write(sender.Id);
                sender.Write(writer);
                Send(writer);
            }
        }
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
                        if (reader.InstanceId != null)
                        {
                            var instanceId = stream.ReadInt(ref readerCtx);

                            if (reader.Log)
                                Debug.Log($"Instance ID: {instanceId} {reader.transform.name}");

                            if (instanceId != reader.InstanceId.Value)
                                continue;
                        }

                        reader.Read(0, stream, ref readerCtx);
                    }
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