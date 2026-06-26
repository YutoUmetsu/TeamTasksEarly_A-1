using UnityEngine;

public class BombPowerManager : MonoBehaviour
{
    public static BombPowerManager Instance { get; private set; }

    [Header("スキルツリーで増えた爆弾の追加ダメージ（基礎攻撃力へのプラス分）")]
    public int bonusDamage = 0;

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

    // 火力アップアイテムを拾った時に呼ぶ関数
    public void IncreaseDamage(int amount)
    {
        bonusDamage += amount;
        Debug.Log($"爆弾の威力ボーナスが +{amount} されました！ 現在のボーナス: {bonusDamage}");
    }
    // ─── 追加：プレステージ用のリセット処理 ───
    public void ResetSkillData()
    {
        bonusDamage = 0;
        Debug.Log("BombPowerManager: 威力ボーナスをリセットしました。");
    }
}