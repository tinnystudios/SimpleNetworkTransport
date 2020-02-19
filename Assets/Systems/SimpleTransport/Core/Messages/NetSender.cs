using System;
using Unity.Networking.Transport;
using UnityEngine;

public abstract class NetSender : MonoBehaviour
{
    public int Id;
    public int? ConnectionId = null;
    public int? InstanceId;

    public abstract DataStreamWriter GetNew();
    public abstract DataStreamWriter Write(DataStreamWriter stream);
}
