using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;

    private float speed;
    [HideInInspector]
    public Vector3 offset = new Vector3(0, 0, -10.0f);

    void Start()
    {
        speed = 10.0f;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, speed);
    }
}
