﻿using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class SpawnSystem : MonoBehaviour
    {
        public NetworkServer Server;

        public List<GhostPair> Ghosts;
        public List<Ghost> Instances;

        private void Awake()
        {
            Server.OnClientConnected += OnClientConnected;
        }

        private void OnClientConnected(NetworkConnection connection)
        {
            SpawnInstances(connection);
            SpawnInServer(0, new Vector3(0,5,0), Quaternion.identity, connection);
        }

        public void SpawnInServer(int prefabId, Vector3 position, Quaternion rotation, NetworkConnection connection)
        {
            SpawnInServer(prefabId, position, rotation, connection.InternalId);
        }

        public void SpawnInServer(int prefabId, Vector3 position, Quaternion rotation, int? connection = null)
        {
            var instance = CreateInstance(prefabId, position, rotation, EOwnershipType.Server);

            // TODO Add optional boolean for server own or not
            if (connection != null)
                AddRPCComponentListenersToServer(instance);

            BroadcastRPCSpawn(instance, connection);
            BroadcastRPCComponents(instance);

            Instances.Add(instance);
        }

        public void SpawnInClient(int prefabId, int instanceId, int ownershipId, Vector3 position, Quaternion rotation, NetworkClient client)
        {
            var type = (EOwnershipType)ownershipId;
            var instance = CreateInstance(prefabId, position, rotation, type);
            instance.transform.name += $"Instance ID: {instanceId} Ownership: {type}";

            if (type == EOwnershipType.Owner)
            {
                var rpcComponents = instance.GetComponentsInChildren<RPCComponent>();
                foreach (var rpcComponent in rpcComponents)
                    client.Add(rpcComponent.GetWriter(instance));
            }
            else
            {
                var rpcComponents = instance.GetComponentsInChildren<RPCComponent>();
                foreach (var rpcComponent in rpcComponents)
                    client.Add(rpcComponent.GetReader(instance));
            }
        }

        public void SpawnRequest(int prefabId, NetworkClient client, Vector3 position, Quaternion rotation)
        {
            var spawnData = new SpawnRPCData
            {
                PrefabId = prefabId,
                Ownership = EOwnershipType.Owner,
                Position = position,
                Rotation = rotation,
            };

            var request = RPCFactory.Create<SpawnRequestRPC, SpawnRPCData>(spawnData);
            client.Write(request);
        }

        public void DespawnInServer(int instanceId)
        {
            var instance = Instances.SingleOrDefault(x => x.GetInstanceID() == instanceId);
            Instances.Remove(instance);

            Server.ClearReferences(instanceId);
            Destroy(instance.gameObject);

            Server.WriteToAllConnections(new DespawnRPC().CreateWriter(instanceId));
        }

        public void SpawnInstances(NetworkConnection connection)
        {
            foreach (var instance in Instances)
            {
                var data = new SpawnRPCData
                {
                    PrefabId = instance.PrefabId,
                    InstanceId = instance.InstanceId,
                    Ownership = EOwnershipType.Server,
                };

                var spawnRPC = RPCFactory.Create<SpawnRPC, SpawnRPCData>(data);
                Server.Write(spawnRPC, connection);
            }
        }

        public Ghost CreateInstance(int prefabId, Vector3 position, Quaternion rotation, EOwnershipType ownership)
        {
            var prefab = ownership == EOwnershipType.Owner ? Ghosts[prefabId].OwnerPrefab : Ghosts[prefabId].GhostPrefab;
            var instance = Instantiate(prefab, transform);
            var instanceId = instance.GetInstanceID();

            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.PrefabId = prefabId;
            instance.InstanceId = instanceId;

            return instance;
        }

        public void AddRPCComponentListenersToServer(Ghost instance)
        {
            var rpcComponents = instance.GetComponentsInChildren<RPCComponent>();
            foreach (var rpcComponent in rpcComponents)
                Server.AddReader(rpcComponent.GetReader(instance));
        }

        public void BroadcastRPCSpawn(Ghost instance, int? connection)
        {
            var ownership = EOwnershipType.Server;

            foreach (var c in Server.Connections)
            {
                if (connection != null)
                    ownership = c.InternalId == connection.Value ? EOwnershipType.Owner : EOwnershipType.Server;

                var spawnRPCData = new SpawnRPCData
                {
                    InstanceId = instance.InstanceId,
                    PrefabId = instance.PrefabId,
                    Ownership = ownership,
                    Position = instance.transform.position,
                    Rotation = instance.transform.rotation,
                };

                var spawnRPC = RPCFactory.Create<SpawnRPC, SpawnRPCData>(spawnRPCData);
                Server.Write(spawnRPC, c);
            }
        }

        public void BroadcastRPCComponents(Ghost instance)
        {
            var broadcastRPCComponents = instance.GetComponentsInChildren<RPCComponent>();
            foreach (var rpcComponent in broadcastRPCComponents)
                Server.AddWriter(rpcComponent.GetWriter(instance));
        }
    }
}