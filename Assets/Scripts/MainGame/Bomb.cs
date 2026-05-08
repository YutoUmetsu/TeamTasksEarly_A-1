using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("判定まとめ用空オブジェクトをセット")]
    public GameObject explosionAreaGroup;

    [Header("爆発演出の持続時間")]
    public float destroyDelay = 0.5f;

    void Start()
    {
        // ゲーム開始時は爆発判定を隠しておく
        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(false);
        }
    }

    // スイッチから呼ばれる関数
    public void Explode()
    {
        Debug.Log("ドカーン！起爆しました");

        // 1. 爆発判定を有効化する
        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(true);
        }

        // 2. 爆弾本体（見た目）を隠す処理
        // もし「爆弾本体」が別オブジェクトなら、ここでSetActive(false)する

        // 3. 少し待ってから爆弾ごと削除
        // (判定が当たった直後に消えないように少し猶予を持たせる)
        Destroy(gameObject, destroyDelay);
    }
}
