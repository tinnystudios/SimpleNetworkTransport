using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class PositionNetSender : NetSender
{
    public bool Log;

    public override DataStreamWriter GetNew() { return new DataStreamWriter(1000, Allocator.Temp); }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        // TODO This should happen when you send in client or server, not in here!
        if (ConnectionId != null)
            writer.Write(ConnectionId.Value);

        if (InstanceId != null)
            writer.Write(InstanceId.Value);

        writer.WriteString($"{transform.position.x},{transform.position.y},{transform.position.z}");

        if (Log)
        {
            Debug.Log($"Sending: {transform.position.x},{transform.position.y},{transform.position.z}");
        }

        return writer;
    }
}