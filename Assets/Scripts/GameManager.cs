using UnityEngine;
using UnityEngine.SceneManagement; // Novo!
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text coinCount;
    public int coin;

    public Transform player; // Novo!
    private PlayerHealth health; // Novo!

    public void Start() // Novo!
    {
        health = FindFirstObjectByType<PlayerHealth>();
    }
    public void Update()
    {
        coinCount.text = coin.ToString();

        if (health.currentHealth <= 0) // Novo!
        {
            GameOver();
        }
        if (player.transform.position.y <= -10)
        {
            GameOver();
        }
    }
    public void AddCoin(int i)
    {
        i = coin;
        coin++;
    }
    public void GameOver()
    {
        SceneManager.LoadScene(0);
    }
}
