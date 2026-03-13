using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float knockbackForce = 7f;
    public float stunDuration = 0.5f;
    public int health = 5;
    public float jumpTimeLimit = 0.3f; // How long can they hold the button?
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    
    private bool isJumping;
    private float jumpCounter;
    private bool isStunned = false;

    private Vector3 characterScale;
    [Header("Sword Setup")]
    public Transform swordPivot;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        characterScale = transform.localScale; // Save your rectangle's original size
    }

    public void SetStun(bool state)
    {
        isStunned = state;
    }
    void Update()
    {
        HandleSwordRotation();
        // 1. START JUMP
        if (Keyboard.current.wKey.wasPressedThisFrame && IsGrounded())
        {
            isJumping = true;
            jumpCounter = jumpTimeLimit;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 2. HOLD JUMP (The "Variable" part)
        if (Keyboard.current.wKey.isPressed && isJumping)
        {
            if (jumpCounter > 0)
            {
                // Keep applying upward velocity while the button is held
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        } else if (Keyboard.current.wKey.wasReleasedThisFrame)
        {
            isJumping = false;
        }

        // 3. RELEASE JUMP
        if (Keyboard.current.wKey.wasReleasedThisFrame)
        {
            isJumping = false;
        }
    }
    void HandleSwordRotation()
    {
        if (swordPivot == null) return;

        // Default: Pointing Right (0 degrees)
        float angle = 0f;

        // 1. Check if S is held for Downward Slash
        if (Keyboard.current.sKey.isPressed)
        {
            angle = -90f; // Point down
        }
        // 2. Otherwise, check if W is held for Upward Slash (Optional)
        else if (Keyboard.current.wKey.isPressed)
        {
            angle = 90f; // Point up
        }
        // 3. Default back to side-facing if neither is pressed
        else
        {
            // We use 0 because the Player's localScale handles the Left/Right flip
            angle = 0f; 
        }

        // Apply the rotation to the pivot
        swordPivot.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void FixedUpdate()
    {
        if (isStunned) return; // Don't allow movement if we're currently stunned
        float horizontal = 0;
        if (Keyboard.current.aKey.isPressed) horizontal = -1;
        if (Keyboard.current.dKey.isPressed) horizontal = 1;
        if (horizontal > 0)
        {
            transform.localScale = characterScale; // Face right    
        } else if (horizontal < 0) {
            transform.localScale = new Vector3(-characterScale.x, characterScale.y, characterScale.z); // Face left
        }
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size * 0.9f, 0f, Vector2.down, .1f, groundLayer);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStunned) return; // Don't take damage if we're already stunned
        if (!collision.CompareTag("Sword") && collision.CompareTag("Enemy")) {
            if (collision.gameObject != gameObject) 
            {
                // If the enemy touched the Player's main body
                takeDamage(collision.transform.position);
            }
        }
    }
    public void takeDamage(Vector3 sourcePosition)
    {
        health -= 1;
        Vector2 knockbackDir = (transform.position - sourcePosition).normalized;
        rb.linearVelocity = Vector2.zero; // Reset current speed for consistent push
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(StunRoutine());

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator StunRoutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }
}