﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class NetworkClient : NetworkClientBase
    {
        public List<INetworkReader> Readers = new List<INetworkReader>();
        public List<INetworkWriter> Writers = new List<INetworkWriter>();

        public ClientConnectionRPC ClientConnectionRpc = new ClientConnectionRPC();

        public int ConnectionId => ClientConnectionRpc.Data;

        private void Awake()
        {
            Readers.Add(ClientConnectionRpc);
            Readers.Add(new SpawnRPC());
        }

        protected override void Update()
        {
            base.Update();

            foreach (var networkWriter in Writers)
            {
                var writer = networkWriter.Write();
                Write(writer);
            }
        }

        protected override void Read(DataStreamReader stream)
        {
            var context = default(DataStreamReader.Context);
            var readerId = stream.ReadInt(ref context);

            var reader = Readers.SingleOrDefault(x => x.Id == readerId);
            reader.Read(stream, ref context);

            // Need to come up with a nicer way
            if (reader is SpawnRPC spawnRpc)
            {
                var spawner = FindObjectOfType<SpawnSystem>();
                var spawnData = spawnRpc.Data;
                spawner.SpawnInClient(spawnData.PrefabId, spawnData.InstanceId, (int)spawnData.Ownership, this);
            }
        }

        public override void Write(DataStreamWriter writer)
        {
            Connection.Send(Driver, writer);
            writer.Dispose();
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