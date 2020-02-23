using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class SpawnRPC : RPC<SpawnRPCData>
    {
        public override int Capacity => 12;
        public override int Id => 3;

        public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
        {
            if(Data == null) Data = new SpawnRPCData();

            Data.InstanceId = reader.ReadInt(ref context);
            Data.PrefabId = reader.ReadInt(ref context);
            Data.Ownership = (EOwnershipType)reader.ReadInt(ref context);

            Debug.Log($"reading: {Data}");
        }

        public override void Write(DataStreamWriter writer, SpawnRPCData data)
        {
            writer.Write(data.InstanceId);
            writer.Write(data.PrefabId);
            writer.Write((int)data.Ownership);

            Debug.Log($"writing: {data}");
        }
    }
}