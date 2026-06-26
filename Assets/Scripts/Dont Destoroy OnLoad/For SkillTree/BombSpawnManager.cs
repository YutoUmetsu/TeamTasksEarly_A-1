using UnityEngine;

public class BombSpawnManager : MonoBehaviour
{
    // どこからでも「BombSpawnManager.Instance」でアクセスできるようにする
    public static BombSpawnManager Instance { get; private set; }

    [Header("スキルツリーで増えた『爆弾ドロップ確率』のボーナス値（％）")]
    public float bonusBombPercent = 0f; // 【変更】個数から確率（%）に変更

    private void Awake()
    {
        // シングルトン重複防止チェック
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ステージが変わってもデータを消さない
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // スキル解放やアイテムを拾った時に、降ってくる確率を増やす関数
    public void IncreaseSpawnBombs(float amount) // 【変更】引数もfloat（％）に
    {
        bonusBombPercent += amount;
        Debug.Log($"爆弾のドロップ確率ボーナスが +{amount}% されました！ 現在のボーナス: {bonusBombPercent}%");
    }
    // ─── 追加：プレステージ用のリセット処理 ───
    public void ResetSkillData()
    {
        bonusBombPercent = 0f;
        Debug.Log("BombSpawnManager: ドロップ確率ボーナスをリセットしました。");
    }
}