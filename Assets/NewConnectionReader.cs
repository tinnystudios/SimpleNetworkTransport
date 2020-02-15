using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class NewConnectionReader : NetReader
{
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var newConnectedId = stream.ReadInt(ref context);
        Debug.Log($"{newConnectedId} has joined");
    }
}

public class GhostNetSender : NetSender
{
    public override DataStreamWriter GetNew()
    {
        return new DataStreamWriter(1000, Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter stream)
    {
        stream.Write(ConnectionId.Value);
        return stream;
        // Prefab ID?
    }
}

public class GhostNetReader : NetReader
{
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var cId = stream.ReadInt(ref context);
    }
}