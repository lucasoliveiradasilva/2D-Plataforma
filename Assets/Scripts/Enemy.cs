using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float velocidade = 2f;
    public float alcance = 3f; // distância máxima para cada lado

    private float pontoInicialX;
    private int direcao = 1; // 1 = direita, -1 = esquerda
    private Rigidbody2D rb;

    public void Start()
    {
        pontoInicialX = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        rb.linearVelocity = new Vector2(velocidade * direcao, rb.linearVelocity.y);

        // Se passou do limite, inverte direção
        if (transform.position.x > pontoInicialX + alcance)
            direcao = -1;
        else if (transform.position.x < pontoInicialX - alcance)
            direcao = 1;

        // Inverte sprite
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direcao;
        transform.localScale = scale;
    }
}
