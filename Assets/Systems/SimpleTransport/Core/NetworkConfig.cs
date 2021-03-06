﻿using Unity.Networking.Transport;
using UnityEngine;

public class NetworkConfig : MonoBehaviour
{
    public bool UseTableau;

    public NetworkEndPoint GetServerEndPoint()
    {
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 9000;

        return endpoint;
    }

    public NetworkEndPoint GetClientEndPoint()
    {
        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;

        if (UseTableau)
            endpoint = NetworkEndPoint.Parse("13.211.83.187", 9000);

        return endpoint;
    }
}