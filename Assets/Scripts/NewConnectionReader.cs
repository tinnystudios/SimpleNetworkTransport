using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

/// <summary>
/// This is read when a connection you do not know of enters.
/// </summary>
public class NewConnectionReader : NetReader
{
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var str = stream.ReadString(ref context);

        if (string.IsNullOrEmpty(str.ToString()))
            return;

        var array = str.ToString().Split(',');

        foreach (var val in array)
        {
            var id = int.Parse(val);
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            var reader = obj.AddComponent<PositionNetReader>();
            reader.Target = obj.transform;
            reader.Id = 4;
            reader.ConnectionId = id;

            var client = GetComponentInParent<ClientBehaviour>();
            client.Readers.Add(reader);
        }

        // The current issue is that new connection reader and writer are adding PositionNetReader because they know about it
        // We want to tie these two together.
        // Also the 'obj' here is not the same as the one the server has. 
        // Maybe the 'GhostCollection' list need to be accessible.
        // Nono, what we need now is a 'prefab' id.
        // The prefab then carries a list of 'components' required to be added.
    }
}
