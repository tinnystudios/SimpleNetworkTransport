using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Server Server;

    public List<GhostPair> Ghosts;
    public List<Ghost> Instances;

    public const int SpawnId = 10;
    public const int SpawnRequestId = 11;
    public const int DespawnRequestId = 12;

    public void SpawnInServer(int prefabId, Vector3 position, Quaternion rotation, NetworkConnection? connection = null)
    {
        var prefab = connection == null ? Ghosts[prefabId].OwnerPrefab : Ghosts[prefabId].GhostPrefab;
        var instance = Instantiate(prefab, transform);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.PrefabId = prefabId;

        var instanceId = instance.GetInstanceID();
        var ownership = EOwnershipType.Server;

        // For now, having a connection means it's owner owned.
        if (connection != null)
        {
            instance.ConnectionId = connection.Value.InternalId;

            foreach (var reader in instance.Readers)
            {
                reader.InstanceId = instanceId;
                Server.AddReader(reader);
            }
        }

        foreach (var sender in instance.Senders)
        {
            sender.InstanceId = instanceId;
            Server.AddSender(sender);
        }

        // If you are the server, notify all clients that a new object has been made
        foreach (var c in Server.Connections)
        {
            if (connection != null)
                ownership = c.InternalId == connection.Value.InternalId ? EOwnershipType.Owner : EOwnershipType.Server;

            var writer = new DataStreamWriter(16, Allocator.Temp);
            writer.Write(SpawnId);
            writer.Write(prefabId);
            writer.Write((int)ownership);
            writer.Write(instanceId);
            Server.Write(writer, c);
        }

        Instances.Add(instance);
    }

    public void SpawnInClient(int prefabId, int instanceId, int ownershipId, ClientBehaviour client)
    {
        var type = (EOwnershipType)ownershipId;
        var prefab = type == EOwnershipType.Owner ? Ghosts[prefabId].OwnerPrefab : Ghosts[prefabId].GhostPrefab;
        var instance = Instantiate(prefab, client.transform);

        if (type == EOwnershipType.Owner)
        {
            foreach (var sender in instance.Senders)
            {
                sender.InstanceId = instanceId;
                client.Senders.Add(sender);
            }
        }
        else
        {
            foreach (var reader in instance.Readers)
            {
                reader.InstanceId = instanceId;
                client.Readers.Add(reader);
            }
        }
    }

    public void DespawnInServer(int instanceId) 
    {
        var instance = Instances.SingleOrDefault(x => x.GetInstanceID() == instanceId);
        Server.ClearReferences(instanceId);
        Destroy(instance.gameObject);
    }

    // Requests goes to the server
    public void RequestSpawn(int prefabId, ClientBehaviour client, Vector3 position, Quaternion rotation)
    {
        var writer = new DataStreamWriter(36, Allocator.Temp);
        writer.Write(SpawnRequestId);
        writer.Write(prefabId);

        writer.Write((int)(position.x * 10000));
        writer.Write((int)(position.y * 10000));
        writer.Write((int)(position.z * 10000));

        writer.Write((int)(rotation.x * 10000));
        writer.Write((int)(rotation.y * 10000));
        writer.Write((int)(rotation.z * 10000));
        writer.Write((int)(rotation.w * 10000));

        client.Send(writer);
    }

    public void RequestDespawn(int instanceId, ClientBehaviour client) 
    {
        var writer = new DataStreamWriter(8, Allocator.Temp);
        writer.Write(DespawnRequestId);
        writer.Write(instanceId);
        client.Send(writer);
    }
}
