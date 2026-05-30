using UnityEngine;

public class BlastAddManager : MonoBehaviour
{
    // ★修正：型名を「BlastAddManager」に変更
    public static BlastAddManager Instance { get; private set; }

    [System.Serializable]
    public class BombBlastData
    {
        public bool hasBlastT1 = false; // 追加爆風Tier1を解放したか
        public bool hasBlastT2 = false; // 追加爆風Tier2を解放したか
    }

    [Header("4種類の爆弾の追加爆風データ")]
    public BombBlastData bombVarticle;
    public BombBlastData bombHorizon;
    public BombBlastData bombCrossT;
    public BombBlastData bombCrossX;

    private void Awake()
    {
        // ─── DontDestroyOnLoad のお決まり設定（ダブり防止） ───
        if (Instance == null)
        {
            // ★修正：自分自身（BlastAddManager）をInstanceに入れる
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}