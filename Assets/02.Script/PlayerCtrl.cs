using UnityEngine;


public class PlayerCtrl : MonoBehaviour
{
    private Rigidbody2D rigid;
    private bool pause;

    [HideInInspector]
    public float speed;
    public float hp;
    public float exp;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        pause = false;
        speed = 3.0f;
        hp = 100.0f;
        exp = 0.0f;
    }

    void Update()
    {
        if(pause)
            return;
        
        if(hp <= 0.0f)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, moveY, 0);
        
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
        rigid.linearVelocity = transform.TransformDirection(moveDirection) * speed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            hp -= 0.1f;
        }

        if(collision.gameObject.tag == "Item")
        {
            exp += 1.0f;
            Destroy(collision.gameObject);
            Debug.Log("Item Get!");
        }
    }
}
