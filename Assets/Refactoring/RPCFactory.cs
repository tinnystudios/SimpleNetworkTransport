using Unity.Networking.Transport;

namespace SimpleTransport
{
    public static class RPCFactory 
    {
        public static TRPC Create<TRPC, T>(T data) where TRPC: RPC<T>, new()
        {
            var rpc = new TRPC();
            rpc.Data = data;
            return rpc;
        }

        public static DataStreamWriter CreateWriter<TRPC, T>(T data, INetwork network) where TRPC : RPC<T>, new()
        {
            var rpc = new TRPC();
            return rpc.CreateWriter(data, network);
        }
    }
}