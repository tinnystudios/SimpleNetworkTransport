using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class SpawnSystem : MonoBehaviour
    {
        public Server Server;

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
            Debug.Log($"Notify all clients to add this ghost");
        }
    }
}