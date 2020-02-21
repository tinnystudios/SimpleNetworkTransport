using UnityEngine;

namespace SimpleTransport
{
    public class ChatSystem : MonoBehaviour
    {
        public NetworkClient NetworkClient;

        public void SendMessage(string text)
        {
            NetworkClient.Write(new ChatRPC().CreateWriter(text));
        }
    }
}