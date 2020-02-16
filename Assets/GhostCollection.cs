using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class GhostCollection : MonoBehaviour
{
    public Server Server;
    public Ghost GhostPrefab;

    public const int newConnectionSenderId = 99;
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

        Write(connection, str);
    }

    public void AddNewClientGhostToConnectedClients(NetworkConnection connection)
    {
        foreach (var c in Server.Connections)
        {
            if (c.InternalId == connection.InternalId)
                continue;

            Write(connection, connection.InternalId.ToString());
        }
    }

    /// <summary>
    /// The writer/sender for NewConnectionReader
    /// </summary>
    public void Write(NetworkConnection connection, string str)
    {
        var writer = new DataStreamWriter(1000000, Allocator.Temp);
        writer.Write(addPreviousGhostSenderId);
        writer.WriteString(str);

        Server.Driver.Send(NetworkPipeline.Null, connection, writer);
        writer.Dispose();
    }

    public void AddNewClientGhostToServer(NetworkConnection connection)
    {
        // Making the ghost on the server side, basically it read position from the new connection
        var player = Instantiate(GhostPrefab);
        var positionReader = player.GetComponent<PositionNetReader>();
        positionReader.ConnectionId = connection.InternalId;
        positionReader.Id = 4;
        positionReader.Target = player.transform;
        Server.AddReader(positionReader);

        // Sending the ghost on the client side, basically it send the new ghost position outwardly to all clients
        var positionSender = player.GetComponent<PositionNetSender>();
        positionSender.ConnectionId = connection.InternalId;
        positionSender.Id = 4;
        Server.AddSender(positionSender);
    }
}
