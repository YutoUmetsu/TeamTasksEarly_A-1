using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("このブロックから手に入るコインの枚数")]
    public int coinRewardAmount = 1;

    [Header("演出用コインのプレハブ")]
    public GameObject coinVisualPrefab;

    public float upwardForce = 5f;
    public float sideForce = 2f;

    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;

        //  追加：自分の配列データを安全にクリアする 
        BlockFall myFallScript = GetComponent<BlockFall>();
        if (myFallScript != null)
        {
            // 自分の座標のデータを完全に「空」にする
            BlockSpawn.blockInfo[myFallScript.myX, myFallScript.myY].blockObj = null;
        }

        // 1. 【即時加算】総量を増やす
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoin(coinRewardAmount);
        }

        // 2. 【演出】コインを跳ね上げさせる
        if (coinVisualPrefab != null)
        {
            GameObject spawnedCoin = Instantiate(coinVisualPrefab, transform.position, Quaternion.identity);
            Rigidbody2D coinRb = spawnedCoin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                float randomX = Random.Range(-sideForce, sideForce);
                Vector2 launchVelocity = new Vector2(randomX, upwardForce);
                coinRb.linearVelocity = launchVelocity;
            }
        }
    }
}