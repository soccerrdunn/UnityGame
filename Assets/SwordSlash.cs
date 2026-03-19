using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    public float slashDuration = 0.15f; // How fast the slash is
    public Vector3 attackOffset = new Vector3(1f, 0, 0); // Where the sword appears
    
    private SpriteRenderer sr;
    private Collider2D swordCollider;
    private float startAngle = 25f;
    private float endAngle = -10f;
    public SpriteRenderer characterSprite;
    public bool hitFloor = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        swordCollider = GetComponent<Collider2D>();
        
        // Ensure sword starts hidden
        sr.enabled = false;
        swordCollider.enabled = false;
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(PerformSlash());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("I hit: " + other.gameObject.name + " with Tag: " + other.tag);
        hitFloor = false; // Reset this every time we hit something, and only set it to true if it's the floor
        if (other.CompareTag("Ground"))
        {
            hitFloor = true;
            Debug.Log("Sword hit the floor!");
        } else if (other.CompareTag("Enemy"))
        {
            // We can also add enemy damage logic here if we want
            Debug.Log("Sword hit an enemy!");
        }
        
    }

    IEnumerator PerformSlash()
    {
        // Look inside the children (the Sword) for the visuals and collider
        SpriteRenderer swordSprite = GetComponentInChildren<SpriteRenderer>();
        Collider2D swordColl = GetComponentInChildren<Collider2D>();
    
        // Check if we actually found them before trying to turn them on
        if (swordSprite != null) swordSprite.enabled = true;
        if (swordColl != null) swordColl.enabled = true;

        float elapsed = 0;
        while (elapsed < slashDuration)
        {
            float direction = characterSprite.flipX ? -1f : 1f;
            if (Keyboard.current.sKey.isPressed)
            {
                startAngle = -70f;
                endAngle = -110f;
            } else if (Keyboard.current.wKey.isPressed)
            {
                startAngle = 70f;
                endAngle = 110f;
            } else
            {
                if (direction > 0) // Facing Right
                {
                    startAngle = 25f;
                    endAngle = -10f;
                }
                else // Facing Left
                {
                    // We mirror the angles by starting at 155 and ending at 190
                    startAngle = 155f;
                    endAngle = 190f;
                }
            }
            // Rotate the PIVOT (this script's object)
            float angle = Mathf.Lerp(startAngle, endAngle, elapsed / slashDuration);
            transform.localRotation = Quaternion.Euler(0, 0, angle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hide them again after the slash is done
        if (swordSprite != null) swordSprite.enabled = false;
        if (swordColl != null) swordColl.enabled = false;
    }
}