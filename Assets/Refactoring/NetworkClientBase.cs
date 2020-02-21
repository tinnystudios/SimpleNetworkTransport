using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public abstract class NetworkClientBase : MonoBehaviour
    {
        public NetworkConfig NetworkConfig;

        protected UdpNetworkDriver Driver;
        protected NetworkConnection Connection;

        private bool _connected;

        private void Start()
        {
            Driver = new UdpNetworkDriver(new INetworkParameter[0]);
            Connect();
        }

        private void OnDestroy()
        {
            Driver.Dispose();
        }

        private void Update()
        {
            Driver.ScheduleUpdate().Complete();

            DataStreamReader stream;
            NetworkEvent.Type cmd;

            while ((cmd = Connection.PopEvent(Driver, out stream)) != NetworkEvent.Type.Empty)
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
            Connection = default(NetworkConnection);
            var endpoint = NetworkConfig.GetClientEndPoint();
            Connection = Driver.Connect(endpoint);
        }

        protected abstract void Read(DataStreamReader stream);
        public abstract void Write(DataStreamWriter writer);
    }
}