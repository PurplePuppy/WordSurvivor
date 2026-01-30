using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("플레이어 이동 속도")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
        {
            Debug.LogWarning("PlayerMovement: No Rigidbody2D found, will use Transform-based movement");
        }
    }

    void Update()
    {
        // WASD / Arrow keys로 입력 받기
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        if (rb)
        {
            rb.velocity = moveInput * moveSpeed;
        }
        else
        {
            // Rigidbody2D가 없으면 Transform으로 직접 이동
            transform.Translate(moveInput * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
