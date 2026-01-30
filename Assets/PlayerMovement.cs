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

        EnsureVisibleSprite();
    }

    void EnsureVisibleSprite()
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null && renderer.sprite != null) return;

        if (renderer == null)
        {
            renderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Create a simple square sprite so the player is visible without external assets.
        const int size = 32;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var color = new Color32(80, 220, 120, 255);
        var pixels = new Color32[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        texture.SetPixels32(pixels);
        texture.Apply();

        var rect = new Rect(0, 0, size, size);
        var pivot = new Vector2(0.5f, 0.5f);
        renderer.sprite = Sprite.Create(texture, rect, pivot, 32f);
        renderer.sortingOrder = 10;
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
