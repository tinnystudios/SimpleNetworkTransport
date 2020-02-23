using UnityEngine;

namespace SimpleTransport
{
    public class SpawnRPCData
    {
        public int InstanceId { get; set; }
        public int PrefabId { get; set; }
        public EOwnershipType Ownership { get; set; }

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public override string ToString()
        {
            return $"Instance:{InstanceId} PrefabId{PrefabId} Ownership:{Ownership} Position:{Position} Rotation{Rotation}";
        }

    }
}
