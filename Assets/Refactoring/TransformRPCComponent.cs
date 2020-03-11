using UnityEngine;

namespace SimpleTransport
{
    public class TransformRPCComponent : RPCComponent
    {
        public override INetworkReader GetReader(Ghost ghost)
        {
            return new TransformRPC
            {
                Data = new TransformRPCData
                {
                    Target = ghost.transform,
                },
                InstanceId = ghost.InstanceId,
            };
        }

        public override INetworkWriter GetWriter(Ghost ghost, INetwork network)
        {
            var transformRPC = new TransformRPC();
            var transformData = new TransformRPCData { Target = ghost.transform };

            // TODO You shoudn't have to create it for it to be called by the client
            transformRPC.CreateWriter(transformData, network, instanceId: ghost.InstanceId);
            return transformRPC;
        }
    }
}
