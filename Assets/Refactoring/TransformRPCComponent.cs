using UnityEngine;

namespace SimpleTransport
{
    public class TransformRPCComponent : RPCComponent
    {
        public Transform Target;

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
            var transformData = new TransformRPCData { Target = Target == null ? ghost.transform : Target };

            // TODO You shoudn't have to create it for it to be called by the client
            transformRPC.CreateWriter(transformData, network, instanceId: ghost.InstanceId);
            return transformRPC;
        }
    }
}
