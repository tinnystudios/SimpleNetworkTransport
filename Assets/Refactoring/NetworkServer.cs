using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class NetworkServer : NetworkServerBase
    {
        public List<INetworkReader> Readers = new List<INetworkReader>();
        public List<INetworkWriter> Writers = new List<INetworkWriter>();

        public NativeList<NetworkConnection> Connections => m_Connections;

        private int _updatePerSeconds = 60;
        private int _currentFrame = 0;

        private void Awake()
        {
            Readers.Add(new SpawnRequestRPC());
        }

        protected override void OnUpdate()
        {
            var updateFrame = 60/_updatePerSeconds;

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
            Write(networkWriter.Write(), connection);
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