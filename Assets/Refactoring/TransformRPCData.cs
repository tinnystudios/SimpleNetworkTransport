using UnityEngine;

namespace SimpleTransport
{
    public class TransformRPCData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public Transform Target;

        public override string ToString()
        {
            return $"{Target.name} {Position}";
        }
    }
}