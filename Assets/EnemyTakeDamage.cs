using System.Collections;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    
    [Header("Health & Combat")]
    public int health = 4;
    public float knockbackForce = 7f;
    public float stunDuration = 0.5f;

    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool isStunned = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Find the player once at the start
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Freeze rotation so the enemy doesn't tip over
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        // 1. If we are stunned by a hit, don't move!
        if (isStunned || playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 2. Only chase if the player is close enough
        if (distance < detectionRange)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            
            // Move horizontally while keeping gravity's Y velocity
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);

            // 3. Flip the sprite to face the player
            if (direction.x > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // Stop moving if player is too far
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStunned) return; // Don't take damage if already stunned
        // 4. Detect Sword Hit
        if (collision.CompareTag("Sword"))
        {
            TakeDamage(1, collision.transform.position);
        }
    }

    public void TakeDamage(int amount, Vector3 sourcePosition)
    {
        health -= amount;

        // 5. Apply Knockback
        Vector2 knockbackDir = (transform.position - sourcePosition).normalized;
        rb.linearVelocity = Vector2.zero; // Reset current speed for consistent push
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // 6. Start Stun
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
