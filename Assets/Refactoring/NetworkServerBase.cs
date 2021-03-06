﻿using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;

namespace SimpleTransport
{
    public abstract class NetworkServerBase : MonoBehaviour
    {
        public Action<NetworkConnection> OnClientConnected;
        public Action<int> OnClientDisconnected;

        public NetworkConfig NetworkConfig;

        public UdpNetworkDriver m_Driver;
        protected NativeList<NetworkConnection> m_Connections;

        private void Start()
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

        private void Update()
        {
            m_Driver.ScheduleUpdate().Complete();

            // CleanUpConnections
            for (int i = 0; i < m_Connections.Length; i++)
            {
                if (!m_Connections[i].IsCreated)
                {
                    Debug.Log($"Connection {i} has been cleaned up");
                    m_Connections.RemoveAtSwapBack(i);
                    --i;
                }
            }

            NetworkConnection c;
            while ((c = m_Driver.Accept()) != default(NetworkConnection))
            {
                Debug.Log($"New user connected: {c.InternalId}");

                m_Connections.Add(c);
                ClientConnected(c);
                OnClientConnected?.Invoke(c);
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
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        Read(i, stream, ref readerCtx);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.Log("Client disconnected from server");
                        m_Connections[i] = default(NetworkConnection);
                        ClientDisconnected(m_Connections[i].InternalId);
                        OnClientDisconnected?.Invoke(m_Connections[i].InternalId);
                    }
                }
            }

            OnUpdate();
        }

        public void Disconnect(int internalId)
        {
            for (int i = 0; i < m_Connections.Length; i++)
            {
                if (m_Connections[i].InternalId == internalId)
                {
                    m_Driver.Disconnect(m_Connections[i]);
                    ClientDisconnected(internalId);

                    break;
                }
            }

            OnClientDisconnected?.Invoke(internalId);
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

        protected abstract void ClientConnected(NetworkConnection connection);
        protected abstract void ClientDisconnected(int id);
        protected abstract void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context);
        protected abstract void OnUpdate();
    }
}