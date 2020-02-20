using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public NetReader[] Readers => GetComponentsInChildren<NetReader>();
    public NetSender[] Senders => GetComponentsInChildren<NetSender>();

    public int? ConnectionId { get; set; }
    public int PrefabId { get; set; }
    public int InstanceId { get; set; }
}
