using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public abstract class ServerBase : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public bool ClientBuild;
    protected NativeList<NetworkConnection> m_Connections;
    public NetworkConfig NetworkConfig;

    private int _frame;

    void Start ()
    {
        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);

        var endpoint = NetworkConfig.GetServerEndPoint();

        if (m_Driver.Bind(endpoint) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        try
        {
            m_Driver.Dispose();
            m_Connections.Dispose();
        }
        catch
        {
            // Ignored as Unplaying Unity can throw errors here when driver is already destroyed and is harmless
        }
    }

    void Update ()
    {
        m_Driver.ScheduleUpdate().Complete();

        // CleanUpConnections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            NewConnection(c);
            Debug.Log($"New user connected: {c.InternalId}");
        }

 

        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
                Assert.IsTrue(true);

            NetworkEvent.Type cmd;

            var readerCtx = default(DataStreamReader.Context);



            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (m_Connections.Length > 0)
                {
                    var updateFrame = 20;
                    _frame++;

                    if (_frame >= updateFrame)
                    {
                        _frame = 0;
                        Send(ref m_Driver, m_Connections[i]);
                    }
                }

                if (cmd == NetworkEvent.Type.Data)
                {
                    Read(i, stream, ref readerCtx);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    protected abstract void NewConnection(NetworkConnection connection);
    protected abstract void Send(ref UdpNetworkDriver m_Driver, NetworkConnection networkConnection);
    protected abstract void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context);
}