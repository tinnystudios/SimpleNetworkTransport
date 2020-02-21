using UnityEngine;

namespace SimpleTransport
{
    public class ChatSystem : MonoBehaviour
    {
        public NetworkClient NetworkClient;

        public void SendToServer(string text)
        {
            var writer = new ChatRPC().CreateWriter(text, NetworkClient.ConnectionId);
            NetworkClient.Write(writer);
        }
    }
}