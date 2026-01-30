using UnityEngine;


public class PlayerCtrl : MonoBehaviour
{
    private bool pause;

    [HideInInspector]
    public float speed;

    void Start()
    {
        pause = false;
        speed = 3.0f;
    }

    void Update()
    {
        if(pause)
            return;

        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}
