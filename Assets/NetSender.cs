using System;
using Unity.Networking.Transport;
using UnityEngine;

public abstract class NetSender : MonoBehaviour
{
    public int Id;
    public abstract DataStreamWriter GetNew();
    public abstract DataStreamWriter Write(DataStreamWriter stream);
}

public abstract class NetReader : MonoBehaviour
{
    public int Id;
    public int ConnectionId;

    public abstract void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context);
}
