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
                ConnectionId = ghost.ConnectionId,
                InstanceId = ghost.InstanceId,
            };
        }

        public override INetworkWriter GetWriter(Ghost ghost)
        {
            var transformRPC = new TransformRPC();
            var transformData = new TransformRPCData { Target = ghost.transform };

            Debug.Log("Adding ghost instance id " + ghost.InstanceId + " Con ID: " + ghost.ConnectionId);

            // TODO You shoudn't have to create it for it to be called by the client
            transformRPC.CreateWriter(transformData, ghost.ConnectionId, ghost.InstanceId);
            return transformRPC;
        }
    }
}
