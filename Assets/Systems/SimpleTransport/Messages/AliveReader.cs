using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class AliveReader : NetReader
{
    public Server Server;
    public List<ConnectionAliveModel> ConnectionAlives = new List<ConnectionAliveModel>();

    private Dictionary<int, ConnectionAliveModel> _connectionAliveMap = new Dictionary<int, ConnectionAliveModel>();

    private void Awake()
    {
        Server.OnClientConnected += AddConnection;
    }

    private void AddConnection(NetworkConnection c)
    {
        var connection = new ConnectionAliveModel() { InternalId = c.InternalId };
        ConnectionAlives.Add(connection);

        _connectionAliveMap.Add(connection.InternalId, connection);
    }

    private void Update()
    {
        for (int i = 0; i < ConnectionAlives.Count; i++) 
        {
            var connection = ConnectionAlives[i];
            var timeElapsed = Time.time - connection.LastMessageReceived;
            if (timeElapsed > 3) 
            {
                Debug.Log("Connection timeout");
                Server.Disconnect(connection.InternalId);
                ConnectionAlives.Remove(connection);
                i--;
            }
        }
    }

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var alive = stream.ReadInt(ref context);
        _connectionAliveMap[connectionId].LastMessageReceived = Time.time;
    }
}

public class ConnectionAliveModel 
{
    public float LastMessageReceived = Time.time;
    public int InternalId;
}
