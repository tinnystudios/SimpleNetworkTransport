using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public abstract class NetworkClientBase : MonoBehaviour
    {
        public NetworkConfig NetworkConfig;

        private UdpNetworkDriver _driver;
        private NetworkConnection _connection;

        private bool _connected;

        private void Start()
        {
            _driver = new UdpNetworkDriver(new INetworkParameter[0]);
            Connect();
        }

        private void OnDestroy()
        {
            _driver.Dispose();
        }

        private void Update()
        {
            _driver.ScheduleUpdate().Complete();

            DataStreamReader stream;
            NetworkEvent.Type cmd;

            while ((cmd = _connection.PopEvent(_driver, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("We are now connected to the server");
                    _connected = true;
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    Read(stream);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected from server");
                }
            }
        }

        public void Connect()
        {
            _connection = default(NetworkConnection);
            var endpoint = NetworkConfig.GetClientEndPoint();
            _connection = _driver.Connect(endpoint);
        }

        protected abstract void Read(DataStreamReader stream);
        protected abstract void Write(DataStreamWriter writer, UdpNetworkDriver driver, NetworkConnection connection);
    }
}