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
        Debug.Log("Currently logged in: " + str); 
    }
}
