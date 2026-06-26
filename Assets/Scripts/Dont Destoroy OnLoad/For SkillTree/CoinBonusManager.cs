using UnityEngine;

public class CoinBonusManager : MonoBehaviour
{
    public static CoinBonusManager Instance { get; private set; }

    [Header("スキルツリーで増えたコインの追加獲得数")]
    public int bonusCoinAmount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseCoinBonus(int amount)
    {
        bonusCoinAmount += amount;
        Debug.Log($"コインの獲得ボーナスが +{amount} されました！ 現在のボーナス: {bonusCoinAmount}");
    }
    // ─── 追加：プレステージ用のリセット処理 ───
    public void ResetSkillData()
    {
        bonusCoinAmount = 0;
        Debug.Log("CoinBonusManager: コイン獲得ボーナスをリセットしました。");
    }
}