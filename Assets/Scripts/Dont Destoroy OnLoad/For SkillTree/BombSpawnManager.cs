using UnityEngine;

public class BombSpawnManager : MonoBehaviour
{
    // どこからでも「BombSpawnManager.Instance」でアクセスできるようにする
    public static BombSpawnManager Instance { get; private set; }

    [Header("スキルツリーで増えた『降ってくる爆弾数』のボーナス値")]
    public int bonusSpawnBombs = 0;

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

    // スキル解放やアイテムを拾った時に、降ってくる数を増やす関数
    public void IncreaseSpawnBombs(int amount)
    {
        bonusSpawnBombs += amount;
        Debug.Log($"降ってくる爆弾の補充数ボーナスが +{amount} されました！ 現在のボーナス: {bonusSpawnBombs}");
    }
}