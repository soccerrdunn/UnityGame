using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class HideOnStart : MonoBehaviour
{
    private SpriteRenderer sr;
    private Collider2D swordCollider;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        swordCollider = GetComponent<Collider2D>();
        sr.enabled = false;
        swordCollider.enabled = false;
    }
}
