using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody Rigidbody;

    private void Start()
    {
        Rigidbody.velocity = Vector3.right * 2;
    }

    private void OnTriggerEnter(Collider collider)
    {

    }
}