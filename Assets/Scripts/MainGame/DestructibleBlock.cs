using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("このブロックから手に入るコインの枚数")]
    public int coinRewardAmount = 1;

    [Header("演出用コインのプレハブ（ただ跳ねるだけの見た目用）")]
    public GameObject coinVisualPrefab;

    [Header("コインが飛び出す勢い（上方向）")]
    public float upwardForce = 5f;
    [Header("コインが飛び出す勢い（横方向の散らばり）")]
    public float sideForce = 2f;

    // ※ExplosionHit.csのリストに、このブロックのプレハブ名を入れておいてください
    void OnDestroy()
    {
        // アプリ終了時や、ステージ切り替え時に勝手にコインが出ないようにする安全弁
        if (!gameObject.scene.isLoaded) return;

        // 1. 【即時加算】壊れた瞬間に、インスペクターで設定した枚数を直接加算する！
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoin(coinRewardAmount);
        }

        // 2. 【演出】コインの見た目だけを「ぴょん」と飛び出させる
        if (coinVisualPrefab != null)
        {
            GameObject spawnedCoin = Instantiate(coinVisualPrefab, transform.position, Quaternion.identity);

            Rigidbody2D coinRb = spawnedCoin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                float randomX = Random.Range(-sideForce, sideForce);
                Vector2 launchVelocity = new Vector2(randomX, upwardForce);

                coinRb.linearVelocity = launchVelocity; // Unity2023以降は linearVelocity / 古い場合は velocity
            }
        }
    }
}