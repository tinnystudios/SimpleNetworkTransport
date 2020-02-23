using System.Linq;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using System;

public class GhostCollection : MonoBehaviour
{
    public Server Server;
    public Spawner Spawner;

    public const int AddPreviousGhostSenderId = 100;

    private void Awake()
    {
        Server.OnClientConnected += OnClientConnected;
        Server.OnClientDisconnected += OnDisconnected;
    }

    private void OnDisconnected(int id)
    {

    }

    private void OnClientConnected(NetworkConnection connection)
    {
        Spawner.SpawnInServer(0, new Vector3(0, 3, 0), Quaternion.identity, connection);
        AddConnectedGhostClientsToNewClient(connection);
    }

    public void NewInClient(int prefabId, int instanceId, int ownershipId, ClientBehaviour client)
    {
        Spawner.SpawnInClient(prefabId, instanceId, ownershipId, client);
    }

    public void AddConnectedGhostClientsToNewClient(NetworkConnection connection)
    {
        var length = Server.Connections.Length;
        var size = 8 + (8 * length);
        var writer = new DataStreamWriter(size, Allocator.Temp);
        writer.Write(AddPreviousGhostSenderId);
        writer.Write(length - 1);

        /*
        foreach (var c in Server.Connections)
        {
            if (c.InternalId == connection.InternalId)
                continue;

            var ghost = Spawner.Instances.SingleOrDefault(x => x.ConnectionId == c.InternalId);
            writer.Write(ghost.PrefabId);
            writer.Write(ghost.GetInstanceID());

        }
        */

        Server.Driver.Send(NetworkPipeline.Null, connection, writer);
        writer.Dispose();
    }
}
