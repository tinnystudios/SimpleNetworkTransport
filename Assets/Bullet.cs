using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody.velocity = Vector3.right * 2;
    }
}
