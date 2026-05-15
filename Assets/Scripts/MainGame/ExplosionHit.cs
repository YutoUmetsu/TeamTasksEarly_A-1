using UnityEngine;
using System.Collections.Generic;

public class ExplosionHit : MonoBehaviour
{
    [Header("爆風で消し去りたいオブジェクトの名前リスト")]
    public List<string> targetNames = new List<string>();

    void OnTriggerEnter2D(Collider2D other)
    {
        string hitObjectName = other.gameObject.name;

        foreach (string targetName in targetNames)
        {
            if (string.IsNullOrEmpty(targetName)) continue;

            if (hitObjectName.StartsWith(targetName))
            {
                Debug.Log($"爆風がヒット！ {hitObjectName} の消滅処理を開始。");

                // 直接 Destroy せず、BlockFall経由で管理データ(deleted)を書き換える
                BlockFall blockFall = other.gameObject.GetComponent<BlockFall>();
                if (blockFall != null)
                {
                    // BlockSpawnの配列データを「deleted」に書き換える！
                    // これにより、BlockSpawnのUpdate側で安全にDestroyされ、OnDestroyのコイン加算が走ります
                    BlockSpawn.blockInfo[blockFall.myX, blockFall.myY].blocks = Blocks.deleted;
                }
                else
                {
                    // もしBlockFallがついていない別のターゲット（敵など）なら、今まで通り直接消す
                    Destroy(other.gameObject);
                }

                break;
            }
        }
    }
}