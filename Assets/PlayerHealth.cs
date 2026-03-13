using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 5;
    public float knockbackForce = 7f;
    public float stunDuration = 0.5f;
    
    private Rigidbody2D rb;
    private PlayerMovement movementScript; // Reference to stop movement
    private bool isInvincible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            // Use collision.transform because OnCollisionEnter2D uses Collision2D, not Collider2D
            TakeDamage(collision.transform.position);
        }
    }

    public void TakeDamage(Vector3 sourcePosition)
    {
        health -= 1;
        Debug.Log("Player Health: " + health);

        // 3. Apply Knockback
        Vector2 knockbackDir = (transform.position - sourcePosition).normalized;
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // 4. Start Stun/Invincibility
        StartCoroutine(StunRoutine());

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator StunRoutine()
    {
        isInvincible = true;
        
        // Tell the movement script to stop taking input
        if (movementScript != null) movementScript.SetStun(true);

        yield return new WaitForSeconds(stunDuration);

        if (movementScript != null) movementScript.SetStun(false);
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Reload scene or show Game Over here
        Destroy(gameObject);
    }
}