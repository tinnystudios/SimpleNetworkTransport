using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public Rigidbody Rigidbody;

    private void Update()
    {
        var dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.position += Time.deltaTime * 3 * dir;

        if (dir.sqrMagnitude > 0.3F)
            transform.forward = dir;

        if (Input.GetKeyUp(KeyCode.Space))
            Rigidbody.velocity = new Vector3(0, 6, 0);
    }
}