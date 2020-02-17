using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Server Server;

    public List<GhostPair> Ghosts;

    public const int SpawnId = 10;
    public const int SpawnRequestId = 11;

    public void SpawnInServer(int prefabId, Vector3 position, Quaternion rotation, NetworkConnection? connection = null)
    {
        var prefab = connection == null ? Ghosts[prefabId].OwnerPrefab : Ghosts[prefabId].GhostPrefab;
        var instance = Instantiate(prefab, transform);
        instance.transform.position = position;
        instance.transform.rotation = rotation;

        var instanceId = instance.GetInstanceID();
        var ownership = EOwnershipType.Server;

        // For now, having a connection means it's owner owned.
        if (connection != null)
        {
            foreach (var reader in instance.Readers)
            {
                reader.InstanceId = instanceId;
                Server.AddReader(reader);
            }
        }
        else
        {
            foreach (var sender in instance.Senders)
            {
                sender.InstanceId = instanceId;
                Server.AddSender(sender);
            }
        }

        // If you are the server, notify all clients that a new object has been made
        foreach (var c in Server.Connections)
        {
            if (connection != null)
                ownership = c.InternalId == connection.Value.InternalId ? EOwnershipType.Owner : EOwnershipType.Server;

            var writer = new DataStreamWriter(1000000, Allocator.Temp);
            writer.Write(SpawnId);
            writer.Write(prefabId);
            writer.Write((int)ownership);
            writer.Write(instanceId);
            Server.Write(writer, c);
        }
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

    public void RequestSpawn(int prefabId, ClientBehaviour client, Vector3 position, Quaternion rotation)
    {
        var writer = new DataStreamWriter(10000, Allocator.Temp);
        writer.Write(SpawnRequestId);
        writer.Write(prefabId);

        // The server should make this instead.
        writer.Write(position.x);
        writer.Write(position.y);
        writer.Write(position.z);

        writer.Write(rotation.x);
        writer.Write(rotation.y);
        writer.Write(rotation.z);
        writer.Write(rotation.w);

        client.Send(writer);
    }
}

public enum EOwnershipType
{
    Owner,
    Server
}