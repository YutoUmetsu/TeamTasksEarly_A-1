using UnityEngine;

public class BombInventoryManager : MonoBehaviour
{
    // どこからでも「BombInventoryManager.Instance」でアクセスできるようにする
    public static BombInventoryManager Instance { get; private set; }

    [Header("スキルツリーで増えた設置上限のボーナス値")]
    public int bonusMaxBombs = 0;

    private void Awake()
    {
        // お決まりのシングルトン重複防止チェック
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

    // アイテムを拾った時などに上限を増やす関数
    public void IncreaseMaxBombs(int amount)
    {
        bonusMaxBombs += amount;
        Debug.Log($"爆弾の設置上限ボーナスが +{amount} されました！ 現在のボーナス: {bonusMaxBombs}");
    }
    // ─── 追加：プレステージ用のリセット処理 ───
    public void ResetSkillData()
    {
        bonusMaxBombs = 0;
        Debug.Log("BombInventoryManager: 設置上限ボーナスをリセットしました。");
    }
}