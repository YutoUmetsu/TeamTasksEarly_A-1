using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    // コインの総量のみを管理
    public int TotalCoins { get; private set; } = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // コインを増やす処理
    public void AddCoin(int amount)
    {
        TotalCoins += amount;
        Debug.Log($"現在の総量: {TotalCoins}枚");
    }
}