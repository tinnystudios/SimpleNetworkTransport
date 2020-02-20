using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;

public abstract class ClientBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;

    protected bool m_Connected;

    public List<NetSender> Senders;
    public List<NetReader> Readers;

    public NetworkConfig NetworkConfig;

    public abstract void SendLoop();
    public abstract void Write(DataStreamReader stream);

    void Start ()
    {
        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        Connect();
    }

    void Update()
    {
        if (m_Connected)
            SendLoop();

        m_Driver.ScheduleUpdate().Complete();

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
                Write(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
            }
        }
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    public void Connect()
    {
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkConfig.GetClientEndPoint();
        m_Connection = m_Driver.Connect(endpoint);
    }

    public void Send(DataStreamWriter writer)
    {
        m_Connection.Send(m_Driver, writer);
        writer.Dispose();
    }
}