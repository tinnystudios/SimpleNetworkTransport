using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class NetworkClient : NetworkClientBase, INetwork
    {
        public List<INetworkReader> Readers = new List<INetworkReader>();
        public List<INetworkWriter> Writers = new List<INetworkWriter>();

        public ClientConnectionRPC ClientConnectionRpc = new ClientConnectionRPC();

        public int ConnectionId => ClientConnectionRpc.Data;

        public int CurrentTick { get; private set; }

        public int TicksPerSecond = 60;
        private int _currentFrame = 0;

        private void Awake()
        {
            Readers.Add(ClientConnectionRpc);
            Readers.Add(new SpawnRPC());
        }

        protected override void Update()
        {
            base.Update();

            foreach (var reader in Readers)
            {
                if (reader is INetworkUpdate u)
                {
                    u.Update();
                }
            }

            var updateFrame = 60 / TicksPerSecond;

            _currentFrame++;
            if (_currentFrame > updateFrame)
            {
                Tick();
                _currentFrame = 0;
            }
        }

        public void Tick()
        {
            foreach (var networkWriter in Writers)
            {
                var writer = networkWriter.Write(this);
                Write(writer);
            }
        }

        protected override void Read(DataStreamReader stream)
        {
            var readerLength = Readers.Count;

            for (int i = 0; i < readerLength; i++)
            {

                INetworkReader reader = Readers[i];
                var context = default(DataStreamReader.Context);
                var readerId = stream.ReadInt(ref context);

                if (reader.Id != readerId)
                    continue;

                //Debug.Log($"{reader.Id}-{reader.InstanceId}");

                if (reader.ConnectionId != null)
                {
                    var conId = stream.ReadInt(ref context);
                }

                if (reader.InstanceId != null)
                {
                    var instanceId = stream.ReadInt(ref context);
                    if (reader.InstanceId != instanceId)
                        continue;
                }

                reader.Read(stream, ref context);

                // Need to come up with a nicer way
                if (reader is SpawnRPC spawnRpc)
                {
                    var spawner = FindObjectOfType<SpawnSystem>();
                    var spawnData = spawnRpc.Data;
                    var position = spawnData.Position;
                    var rotation = spawnData.Rotation;

                    spawner.SpawnInClient(spawnData.PrefabId, spawnData.InstanceId, (int)spawnData.Ownership, position, rotation, this);
                }
            }
        }

        public override void Write(DataStreamWriter writer)
        {
            Connection.Send(Driver, writer);
            writer.Dispose();
        }

        public void Write(INetworkWriter networkWriter) 
        {
            Write(networkWriter.Write(this));
        }

        public void Add(INetworkWriter writer)
        {
            Writers.Add(writer);
        }

        public void Add(INetworkReader reader)
        {
            Readers.Add(reader);
        }
    }
}