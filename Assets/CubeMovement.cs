using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    private void Update()
    {
        var dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.position += Time.deltaTime * 3 * dir;
    }
}