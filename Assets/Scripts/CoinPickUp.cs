using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    public GameManager gm;

    public void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gm.AddCoin(1);
            Destroy(gameObject);
        }
    }
}
