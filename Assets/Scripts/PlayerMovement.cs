using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float knockbackForce = 10f;
    private float invincibleTime = 0.5f;
    private float lastHorizontal = 1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;

    private bool canTakeDamage = true;
    private bool isGrounded;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    public void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        if (move != 0)
            lastHorizontal = move;

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
        if (collision.collider.CompareTag("Spike") && canTakeDamage)
        {
            // Direção contrária à última movimentação
            Vector2 knockDir = new Vector2(-Mathf.Sign(lastHorizontal), 0);

            StartCoroutine(ReactToSpike(knockDir));
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
    private IEnumerator ReactToSpike(Vector2 knockDir)
    {
        canTakeDamage = false;
        spriteRenderer.color = Color.white;

        CameraShake.Instance.Shake(0.2f, 0.3f); // Shake levinho


        rb.simulated = false; // Desliga a física pra não bugar colisão

        float knockDuration = 0.2f;
        float timer = 0f;
        float knockSpeed = knockbackForce;

        while (timer < knockDuration)
        {
            transform.position += (Vector3)(knockDir * knockSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Reativa física
        rb.simulated = true;

        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(invincibleTime);

        canTakeDamage = true;
    }

    //CTRL+S = Salvar!
}