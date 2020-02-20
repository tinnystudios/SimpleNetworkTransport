using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class Client : ClientBehaviour
{
    public override void SendLoop()
    {
        foreach (var sender in Senders)
        {
            var writer = sender.GetNew();
            writer.Write(sender.Id);
            sender.Write(writer);
            Send(writer);
        }
    }

    public override void Read(DataStreamReader stream)
    {
        var readers = new List<NetReader>(Readers);
        foreach (var reader in readers)
        {
            var readerCtx = default(DataStreamReader.Context);

            var id = stream.ReadInt(ref readerCtx);
            if (id == reader.Id)
            {
                if (reader.InstanceId != null)
                {
                    var instanceId = stream.ReadInt(ref readerCtx);

                    if (reader.Log)
                        Debug.Log($"Instance ID: {instanceId} {reader.transform.name}");

                    if (instanceId != reader.InstanceId.Value)
                        continue;
                }

                reader.Read(0, stream, ref readerCtx);
            }
        }
    }

    public void ClearReferences(int instanceId)
    {
        for (int i = 0; i < Senders.Count; i++)
        {
            if (Senders[i].InstanceId == instanceId)
            {
                Senders.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < Readers.Count; i++)
        {
            if (Readers[i].InstanceId == instanceId)
            {
                Readers.RemoveAt(i);
                i--;
            }
        }
    }
}
