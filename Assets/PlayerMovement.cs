using UnityEngine;
using UnityEngine.InputSystem;

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
            Debug.LogError("PlayerMovement: Rigidbody2D is required for collision-based movement.");
        }
    }

    void Update()
    {
        // Input System: WASD / Arrow keys
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1f;

        moveInput = new Vector2(x, y);
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }
    }

    void FixedUpdate()
    {
        if (!rb) return;

        rb.linearVelocity = moveInput * moveSpeed;
    }
}
