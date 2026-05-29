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

    /// <summary>
    /// 指定された枚数のコインを消費する。足りていれば減らしてtrue、足りなければfalseを返す。
    /// </summary>
    public bool TrySpendCoins(int amount)
    {
        // もし持っているコインが足りなかったら、消費させずに失敗を返す
        if (TotalCoins < amount)
        {
            Debug.LogWarning($"コインが足りません！ 必要: {amount}枚 / 所持: {TotalCoins}枚");
            return false;
        }

        // コインが足りているので減らす
        TotalCoins -= amount;
        Debug.Log($"コインを {amount} 枚消費しました。 現在の総量: {TotalCoins}枚");
        return true;
    }
}