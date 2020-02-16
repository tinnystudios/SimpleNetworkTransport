using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class GhostCollection : MonoBehaviour
{
    public Server Server;
    public Ghost GhostPrefab;

    public const int NewClientGhostSenderId = 99;
    public const int addPreviousGhostSenderId = 100;

    private void Awake()
    {
        Server.OnNewConnection += OnNewConnection;
    }

    private void OnNewConnection(NetworkConnection connection)
    {
        AddNewClientGhostToServer(connection);
        AddNewClientGhostToConnectedClients(connection);
        AddConnectedGhostClientsToNewClient(connection);
    }

    public void AddConnectedGhostClientsToNewClient(NetworkConnection connection)
    {
        var str = "";
        var count = 0;

        foreach (var c in Server.Connections)
        {
            count++;

            if (c.InternalId == connection.InternalId)
                continue;

            str += $"{c.InternalId}";

            if (count < Server.Connections.Length - 1)
                str += ",";
        }

        Write(connection, str, addPreviousGhostSenderId);
    }

    public void AddNewClientGhostToConnectedClients(NetworkConnection connection)
    {
        foreach (var c in Server.Connections)
        {
            if (c.InternalId == connection.InternalId)
                continue;

            Write(c, connection.InternalId, NewClientGhostSenderId);
        }
    }

    /// <summary>
    /// The writer/sender for NewConnectionReader
    /// </summary>
    public void Write(NetworkConnection connection, string str, int senderId)
    {
        var writer = new DataStreamWriter(1000000, Allocator.Temp);
        writer.Write(senderId);
        writer.WriteString(str);

        Server.Driver.Send(NetworkPipeline.Null, connection, writer);
        writer.Dispose();
    }

    /// <summary>
    /// The writer/sender for NewConnectionReader
    /// </summary>
    public void Write(NetworkConnection connection, int val, int senderId)
    {
        var writer = new DataStreamWriter(1000000, Allocator.Temp);
        writer.Write(senderId);
        writer.Write(val);

        Server.Driver.Send(NetworkPipeline.Null, connection, writer);
        writer.Dispose();
    }

    public void AddNewClientGhostToServer(NetworkConnection connection)
    {
        var player = Instantiate(GhostPrefab, transform);
        var readers = player.GetComponentsInChildren<NetReader>();
        foreach (var reader in readers)
        {
            reader.ConnectionId = connection.InternalId;
            Server.AddReader(reader);
        }

        var senders = player.GetComponentsInChildren<NetSender>();
        foreach (var sender in senders)
        {
            sender.ConnectionId = connection.InternalId;
            Server.AddSender(sender);
        }
    }

    public Ghost NewGhost(ClientBehaviour client, int connectionId)
    {
        var ghost = Instantiate(GhostPrefab, client.transform);
        ghost.transform.name = $"Ghost: {connectionId}";

        var reader = ghost.GetComponent<PositionNetReader>();
        reader.ConnectionId = connectionId;

        client.Readers.Add(reader);
        return ghost;
    }
}
