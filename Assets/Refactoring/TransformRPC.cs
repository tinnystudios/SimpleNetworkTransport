using System;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    [Serializable]
    public class TransformRPC : RPC<TransformRPCData>, INetworkUpdate
    {
        public float InterpolationFactor = 3;

        public override int Capacity => 36;
        public override int Id => 4;

        private Vector3? _currentPosition;
        private Vector3? _targetPosition;

        public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
        {
            byte[] buff = reader.ReadBytesAsArray(ref context, sizeof(float) * 7);
            Vector3 position = Vector3.zero;

            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            position.z = BitConverter.ToSingle(buff, 2 * sizeof(float));

            var rotation = Quaternion.identity;
            rotation.x = BitConverter.ToSingle(buff, 3 * sizeof(float));
            rotation.y = BitConverter.ToSingle(buff, 4 * sizeof(float));
            rotation.z = BitConverter.ToSingle(buff, 5 * sizeof(float));
            rotation.w = BitConverter.ToSingle(buff, 6 * sizeof(float));

            Data.Target.rotation = rotation;

            if (_currentPosition == null)
                _currentPosition = position;
            else 
            {
                var dist = Vector3.Distance(_currentPosition.Value, position);

                if (dist > 0.01f)
                {
                    var newPosition = position;
                    var lastPosition = _currentPosition;
                    var dir = newPosition - lastPosition;

                    _currentPosition = position;

                    var predictedPosition = newPosition + dir * 1.1F;
                    position = predictedPosition.Value;
                }
                else 
                {
                    position = _currentPosition.Value;
                }
            }

            _targetPosition = position;

            //Debug.Log($"Read: {Data}");
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

            Data.Position = position;
            Data.Rotation = rotation;

            writer.Write(buff);
        }

        public void Update()
        {
            if(_targetPosition != null)
                Data.Target.position = Vector3.Lerp(Data.Target.position, _targetPosition.Value, InterpolationFactor * Time.deltaTime);
        }
    }
}