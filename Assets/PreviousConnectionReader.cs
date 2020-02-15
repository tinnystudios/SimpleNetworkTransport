using Unity.Networking.Transport;
using UnityEngine;

/// <summary>
/// Add ghosts
/// </summary>
public class PreviousConnectionReader : NetReader
{
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var str = stream.ReadString(ref context);

        if (string.IsNullOrEmpty(str.ToString()))
            return;

        var array = str.ToString().Split(',');
        
        foreach(var val in array)
        {
            var id = int.Parse(val);

            var obj = new GameObject();
            var reader = obj.AddComponent<PositionNetReader>();
            reader.Target = obj.transform;
            reader.Id = 4;
            reader.ConnectionId = id;

            Debug.Log("Added Position Reader for : " + id);

            var client = GetComponentInParent<ClientBehaviour>();
            client.Readers.Add(reader);
        }
    }
}
