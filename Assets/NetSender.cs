using System;
using Unity.Networking.Transport;
using UnityEngine;

public abstract class NetSender : MonoBehaviour
{
    public abstract DataStreamWriter Write();
}

public abstract class NetReader : MonoBehaviour
{
    public abstract void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context);
}
