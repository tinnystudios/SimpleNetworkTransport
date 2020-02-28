using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class NetworkPing 
    {
        public float? SentTime;
        public float? ReceivedTime;
        public float? Difference => ReceivedTime != null ? (ReceivedTime - SentTime) * 1000: null;
    }

    public class NetworkServer : NetworkServerBase, INetwork
    {
        public List<INetworkReader> Readers = new List<INetworkReader>();
        public List<INetworkWriter> Writers = new List<INetworkWriter>();

        public NativeList<NetworkConnection> Connections => m_Connections;

        public int TicksPerSecond = 60;
        private int _currentFrame = 0;
        public Dictionary<int, List<NetworkPing>> _pingMap = new Dictionary<int, List<NetworkPing>>();

        public int CurrentTick { get; private set; }

        private void Awake()
        {
            Readers.Add(new SpawnRequestRPC());
            Application.targetFrameRate = 60;
        }

        protected override void OnUpdate()
        {
            foreach (var reader in Readers) 
            {
                if (reader is INetworkUpdate u) 
                {
                    u.Update();
                }
            }

            var updateFrame = 60/TicksPerSecond;

            _currentFrame++;
            if (_currentFrame > updateFrame)
            {
                Tick();
                _currentFrame = 0;
            }
        }

        public void Tick()
        {
            foreach (var connection in m_Connections)
            {
                foreach (var writer in Writers)
                {
                    Write(writer, connection);
                    SuccessfullyWrite(writer, connection);
                }
            }
        }

        protected override void ClientConnected(NetworkConnection connection)
        {
            var writerRPC = RPCFactory.Create<ClientConnectionRPC, int>(connection.InternalId);
            Write(writerRPC, connection);
        }

        protected override void ClientDisconnected(int id)
        {
            CleanReaders(id);
        }

        protected override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
        {
            var readerLength = Readers.Count;

            for (int i = 0; i < readerLength; i++)
            {
                INetworkReader reader = Readers[i];
                var c = default(DataStreamReader.Context);
                var readerId = stream.ReadInt(ref c);

                if (reader.Id != readerId)
                    continue;

                if (reader.ConnectionId != null)
                {
                    var conId = stream.ReadInt(ref c);
                }

                if (reader.InstanceId != null)
                {
                    var instanceId = stream.ReadInt(ref c);
                    if (reader.InstanceId != instanceId)
                        continue;
                }

                reader.Read(stream, ref c);

                // TODO This should be refactored to not have to check
                if (reader is SpawnRequestRPC spawnRpc)
                {
                    var spawner = FindObjectOfType<SpawnSystem>();
                    var spawnData = spawnRpc.Data;

                    // Passing the connection id will make it owned by the connection.
                    spawner.SpawnInServer(spawnData.PrefabId, spawnData.Position, spawnData.Rotation, null);
                }

                SuccessfullyRead(reader, connectionId);
            }
        }

        // Maybe this should be an acknowledgment system instead of just a ping
        private void SuccessfullyWrite(INetworkWriter writer, NetworkConnection connection)
        {
            var networkPing = new NetworkPing{SentTime = Time.time};

            if (!_pingMap.ContainsKey(connection.InternalId)) 
                _pingMap.Add(connection.InternalId, new List<NetworkPing>());

            _pingMap[connection.InternalId].Add(networkPing);
        }

        private void SuccessfullyRead(INetworkReader reader, int connectionId)
        {
            if (!_pingMap.ContainsKey(connectionId))
                return;

            var lastSent = _pingMap[connectionId];
            lastSent[lastSent.Count - 1].ReceivedTime = Time.time;
        }

        private void OnGUI()
        {
            foreach (var connection in Connections) 
            {
                if (_pingMap.ContainsKey(connection.InternalId)) 
                {
                    var pings = _pingMap[connection.InternalId];
                    for (int i = pings.Count - 1; i > 0; i--) 
                    {
                        var ping = pings[i];
                        if (ping.Difference == null)
                            continue;

                        GUILayout.Label($"{ping.Difference}");
                        break;
                    }
                }
            }
        }

        private void CleanReaders(int connectionId)
        {
            for (var i = 0; i < Readers.Count; i++)
            {
                if (Readers[i].ConnectionId != connectionId)
                    continue;

                Readers.RemoveAt(i);
                i--;
            }
        }

        public void Write(INetworkWriter networkWriter, NetworkConnection connection)
        {
            Write(networkWriter.Write(this), connection);
        }

        public void AddReader(INetworkReader reader)
        {
            Readers.Add(reader);
        }

        public void AddWriter(INetworkWriter writer)
        {
            Writers.Add(writer);
        }

        public void ClearReferences(int instanceId)
        {
            throw new NotImplementedException();
        }
    }
}
