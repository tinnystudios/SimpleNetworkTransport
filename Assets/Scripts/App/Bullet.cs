using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody Rigidbody;

    private void Start()
    {
        Rigidbody.velocity = transform.forward * 5;
    }

    private void OnTriggerEnter(Collider collider)
    {
        var player = collider.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage();
        }
    }
}