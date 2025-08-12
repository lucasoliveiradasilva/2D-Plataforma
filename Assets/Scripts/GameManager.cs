using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text coinCount;
    public int coin;
    public void Update()
    {
        coinCount.text = coin.ToString();
    }
    public void AddCoin(int i)
    {
        i = coin;
        coin++;
    }
}
