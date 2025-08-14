using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;
    private float knockbackForce = 10f;
    private float invincibleTime = 0.5f;
    private float lastHorizontal = 1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;
    public ParticleSystem particleHit; // !
    public PlayerHealth health; // !

    private bool canTakeDamage = true;
    private bool isGrounded;
    private bool isKnockback;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        health = FindAnyObjectByType<PlayerHealth>(); // !
    }

    public void Update()
    {
        if (!isKnockback)
        {
            float move = Input.GetAxisRaw("Horizontal");
            if (move != 0)
                lastHorizontal = move;

            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground")) // !
        {
            isGrounded = true;
        }
        if (collision.collider.CompareTag("Spike") && canTakeDamage)
        {
            particleHit.Play(); 
            Vector2 knockDir = new Vector2(-Mathf.Sign(lastHorizontal), 0.5f);
            health.TakeDamage(5);
            StartCoroutine(ReactToSpike(knockDir));
        }
        if (collision.collider.CompareTag("Enemy") && canTakeDamage) // !
        {
            particleHit.Play(); 
            Vector2 knockDir = new Vector2(-Mathf.Sign(lastHorizontal), 0.5f);
            health.TakeDamage(5);
            StartCoroutine(ReactToSpike(knockDir));
        }
    }

    private IEnumerator ReactToSpike(Vector2 knockDir)
    {
        canTakeDamage = false;
        isKnockback = true;

        spriteRenderer.color = Color.white;
        CameraShake.Instance.Shake(0.2f, 0.3f);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);

        isKnockback = false;
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(invincibleTime);
        canTakeDamage = true;
    }
}
