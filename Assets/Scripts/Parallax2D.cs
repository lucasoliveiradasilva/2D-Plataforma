using UnityEngine;

public class Parallax2D : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 ultimaPosicaoCamera;
    [SerializeField] private float fatorParallax = 0.5f; // Fator de movimento do parallax (quanto menor, mais distante parece estar)

    public void Start()
    {
        // Pegamos a posi��o inicial da c�mera
        cameraTransform = Camera.main.transform;
        ultimaPosicaoCamera = cameraTransform.position;
    }

    public void LateUpdate()
    {
        Vector3 movimentoDaCamera = cameraTransform.position - ultimaPosicaoCamera; // Calcula quanto a c�mera se moveu desde o �ltimo frame
        transform.position += movimentoDaCamera * fatorParallax; // Move o fundo baseado no fator de parallax

        ultimaPosicaoCamera = cameraTransform.position;
    }
}
