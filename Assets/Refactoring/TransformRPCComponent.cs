using UnityEngine;

namespace SimpleTransport
{
    public class TransformRPCComponent : RPCComponent
    {
        public Transform Target;

        public TransformRPC RPC;

        public override INetworkReader GetReader(Ghost ghost)
        {
            RPC.Data = new TransformRPCData
            {
                Target = ghost.transform,
            };

            RPC.InstanceId = ghost.InstanceId;

            return RPC;
        }

        public override INetworkWriter GetWriter(Ghost ghost, INetwork network)
        {
            var rpc = new TransformRPC();
            var transformData = new TransformRPCData { Target = Target == null ? ghost.transform : Target};

            // TODO You shoudn't have to create it for it to be called by the client
            rpc.CreateWriter(transformData, network, instanceId: ghost.InstanceId);
            return rpc;
        }
    }
}
