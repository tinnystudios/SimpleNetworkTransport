using UnityEngine;

namespace SimpleTransport
{
    public abstract class RPCComponent : MonoBehaviour
    {
        public abstract INetworkReader GetReader(Ghost ghost);
        public abstract INetworkWriter GetWriter(Ghost ghost, INetwork network);
    }
}