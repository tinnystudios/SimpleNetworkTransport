using System;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class TransformRPC : RPC<TransformRPCData>
    {
        public override int Capacity => 36 + 60;
        public override int Id => 4;

        public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
        {
            byte[] buff = reader.ReadBytesAsArray(ref context, sizeof(float) * 7);
            Vector3 vect = Vector3.zero;

            vect.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            vect.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            vect.z = BitConverter.ToSingle(buff, 2 * sizeof(float));

            var rotation = Quaternion.identity;
            rotation.x = BitConverter.ToSingle(buff, 3 * sizeof(float));
            rotation.y = BitConverter.ToSingle(buff, 4 * sizeof(float));
            rotation.z = BitConverter.ToSingle(buff, 5 * sizeof(float));
            rotation.w = BitConverter.ToSingle(buff, 6 * sizeof(float));

            Debug.Log(vect);

            //Data.Target.position = vect;
            //Data.Target.rotation = rotation;
        }

        public override void Write(DataStreamWriter writer, TransformRPCData data)
        {
            var position = data.Target.position;
            var rotation = data.Target.rotation;

            byte[] buff = new byte[sizeof(float) * 7];
            Buffer.BlockCopy(BitConverter.GetBytes(position.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(position.y), 0, buff, 1 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(position.z), 0, buff, 2 * sizeof(float), sizeof(float));

            Buffer.BlockCopy(BitConverter.GetBytes(rotation.x), 0, buff, 3 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(rotation.y), 0, buff, 4 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(rotation.z), 0, buff, 5 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(rotation.w), 0, buff, 6 * sizeof(float), sizeof(float));

            writer.Write(buff);
        }
    }
}