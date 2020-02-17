using UnityEngine;
using Unity.Networking.Transport;

public class GhostCollection : MonoBehaviour
{
    public Server Server;
    public Spawner Spawner;

    private void Awake()
    {
        Server.OnNewConnection += OnNewConnection;
    }

    private void OnNewConnection(NetworkConnection connection)
    {
        Spawner.SpawnInServer(0, new Vector3(0, 3, 0), Quaternion.identity, connection);
    }
}
