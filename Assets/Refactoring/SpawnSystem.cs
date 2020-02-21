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
            SpawnInServer(0, Vector3.zero, Quaternion.identity, connection);
        }

        public void SpawnInServer(int prefabId, Vector3 position, Quaternion rotation, NetworkConnection? connection = null)
        {
            Debug.Log($"Spawned {prefabId} in server");

            var instance = Instantiate(Ghosts[prefabId].GhostPrefab);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.PrefabId = prefabId;

            var instanceId = instance.GetInstanceID();
            var ownership = EOwnershipType.Server;

            // For now, having a connection means it's owner owned.
            if (connection != null)
            {
                instance.InstanceId = instanceId;
                instance.ConnectionId = connection.Value.InternalId;

                var transformRPC = new TransformRPC
                {
                    Data = new TransformRPCData
                    {
                        Target = instance.transform,
                    },
                    ConnectionId = connection.Value.InternalId,
                    InstanceId = instanceId,
                };

                Server.AddReader(transformRPC);
            }

            // Broadcast to all clients to make this object
            foreach (var c in Server.Connections)
            {
                if (connection != null)
                    ownership = c.InternalId == connection.Value.InternalId ? EOwnershipType.Owner : EOwnershipType.Server;

                var spawnRPCData = new SpawnRPCData
                {
                    InstanceId = instanceId,
                    PrefabId = prefabId,
                    Ownership = ownership
                };

                var spawnWriter = new SpawnRPC().CreateWriter(spawnRPCData);
                Server.Write(spawnWriter, c);
            }

            // TODO Get the components

            // TODO Broadcast this ghost position to all clients

            Instances.Add(instance);
        }

        public void SpawnInClient(int prefabId, int instanceId, int ownershipId, NetworkClient client)
        {
            var type = (EOwnershipType)ownershipId;
            var prefab = type == EOwnershipType.Owner ? Ghosts[prefabId].OwnerPrefab : Ghosts[prefabId].GhostPrefab;
            var instance = Instantiate(prefab, client.transform);

            if (type == EOwnershipType.Owner)
            {
                var transformRPC = new TransformRPC();
                var transformData = new TransformRPCData { Target = instance.transform };

                // TODO You shoudn't have to create it for it to be called by the client
                transformRPC.CreateWriter(transformData, client.ConnectionId, instanceId);
                client.Add(transformRPC);
            }
            else
            {
                foreach (var reader in instance.Readers)
                {
                    reader.InstanceId = instanceId;
                    //client.Readers.Add(reader);
                }
            }

            instance.InstanceId = instanceId;
            Instances.Add(instance);
        }
    }
}