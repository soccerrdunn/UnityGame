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
                startAngle = 25f;
                endAngle = -10f;
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