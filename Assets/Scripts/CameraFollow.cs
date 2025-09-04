using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float yLimit = 2f;

    private void Update()
    {
        float y = transform.position.y; // Guarda a pos da camera.
        if (Mathf.Abs(player.position.y - y) > yLimit) y = player.position.y; // Mede e compara a distancia.
        transform.position = new Vector3(player.position.x, y, transform.position.z); // Atualiza para posição correta!
    }
}