﻿using System;
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
    public const int DespawnClientRequestId = 13;

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
            //instance.ConnectionId = connection.Value.InternalId;

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

        instance.InstanceId = instanceId;
        Instances.Add(instance);
    }

    public void DespawnInClient(int instanceId, Client client)
    {
        for (int i = 0; i < Instances.Count; i++)
        {
            if (Instances[i].InstanceId == instanceId) {
                client.ClearReferences(instanceId);

                Destroy(Instances[i].gameObject);
                Instances.RemoveAt(i);
                break;
            }
        }
    }

    public void DespawnInServer(int instanceId) 
    {
        var instance = Instances.SingleOrDefault(x => x.GetInstanceID() == instanceId);
        Instances.Remove(instance);

        Server.ClearReferences(instanceId);
        Destroy(instance.gameObject);

        foreach (var connection in Server.Connections)
        {
            var writer = new DataStreamWriter(8, Allocator.Temp);
            writer.Write(DespawnClientRequestId);
            writer.Write(instanceId);
            Server.Write(writer, connection);
        }
    }

    // Client requesting to spawn
    public void RequestSpawn(int prefabId, ClientBehaviour client, Vector3 position, Quaternion rotation)
    {
        var writer = new DataStreamWriter(36, Allocator.Temp);
        writer.Write(SpawnRequestId);
        writer.Write(prefabId);

        byte[] buff = new byte[sizeof(float) * 7];
        Buffer.BlockCopy(BitConverter.GetBytes(position.x), 0, buff, 0 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(position.y), 0, buff, 1 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(position.z), 0, buff, 2 * sizeof(float), sizeof(float));

        Buffer.BlockCopy(BitConverter.GetBytes(rotation.x), 0, buff, 3 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(rotation.y), 0, buff, 4 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(rotation.z), 0, buff, 5 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(rotation.w), 0, buff, 6 * sizeof(float), sizeof(float));

        writer.Write(buff);

        client.Send(writer);
    }

    // Client requesting to despawn
    public void RequestDespawn(int instanceId, ClientBehaviour client) 
    {
        var writer = new DataStreamWriter(8, Allocator.Temp);
        writer.Write(DespawnRequestId);
        writer.Write(instanceId);
        client.Send(writer);
    }
}
