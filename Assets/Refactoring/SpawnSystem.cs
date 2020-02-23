using System.Collections.Generic;
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
            foreach (var instance in Instances)
            {
                var spawnRPC = new SpawnRPC();
                var data = new SpawnRPCData
                {
                    PrefabId = instance.PrefabId,
                    InstanceId = instance.InstanceId,
                    Ownership = EOwnershipType.Server,
                };

                var writer = spawnRPC.CreateWriter(data);
                Server.Write(writer, connection);
            }

            SpawnInServer(0, new Vector3(0,5,0), Quaternion.identity, connection);
        }

        public void SpawnInServer(int prefabId, Vector3 position, Quaternion rotation, NetworkConnection connection)
        {
            SpawnInServer(prefabId, position, rotation, connection.InternalId);
        }

        public void SpawnInServer(int prefabId, Vector3 position, Quaternion rotation, int? connection = null)
        {
            var prefab = connection == null ? Ghosts[prefabId].OwnerPrefab : Ghosts[prefabId].GhostPrefab;
            var instance = Instantiate(prefab, transform);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.PrefabId = prefabId;

            var instanceId = instance.GetInstanceID();
            var ownership = EOwnershipType.Server;

            instance.InstanceId = instanceId;

            // For now, having a connection means it's owner owned.
            if (connection != null)
            {
                var rpcComponents = instance.GetComponentsInChildren<RPCComponent>();
                foreach (var rpcComponent in rpcComponents)
                    Server.AddReader(rpcComponent.GetReader(instance));
            }

            // Broadcast to all clients to make this object
            foreach (var c in Server.Connections)
            {
                if (connection != null)
                    ownership = c.InternalId == connection.Value ? EOwnershipType.Owner : EOwnershipType.Server;

                var spawnRPCData = new SpawnRPCData
                {
                    InstanceId = instanceId,
                    PrefabId = prefabId,
                    Ownership = ownership,
                    Position = position,
                    Rotation = rotation,
                };

                var spawnWriter = new SpawnRPC().CreateWriter(spawnRPCData);
                Server.Write(spawnWriter, c);
            }

            var cs = instance.GetComponentsInChildren<RPCComponent>();
            foreach (var rpcComponent in cs)
            {
                Server.AddWriter(rpcComponent.GetWriter(instance));
            }

            // TODO Broadcast this ghost position to all clients

            Instances.Add(instance);
        }

        public void SpawnInClient(int prefabId, int instanceId, int ownershipId, Vector3 position, Quaternion rotation, NetworkClient client)
        {
            var type = (EOwnershipType)ownershipId;
            var prefab = type == EOwnershipType.Owner ? Ghosts[prefabId].OwnerPrefab : Ghosts[prefabId].GhostPrefab;

            // TODO Configuring the instance here determiens if it needs to read connection id or not
            var instance = Instantiate(prefab, client.transform);

            instance.InstanceId = instanceId;

            instance.transform.position = position;
            instance.transform.rotation = rotation;

            instance.transform.name += $"Instance ID: {instanceId} Ownership: {type}";

            // TODO Ghost for non-owners are not reading correctly.

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
            var request = new SpawnRequestRPC();
            var spawnData = new SpawnRPCData
            {
                PrefabId = prefabId,
                Ownership = EOwnershipType.Owner,
                Position = position,
                Rotation = rotation,
            };

            var writer = request.CreateWriter(spawnData);

            client.Write(writer);
        }
    }
}