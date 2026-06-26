using UnityEngine;
using System.Collections.Generic;

public class ExplosionHit : MonoBehaviour
{
    [Header("爆風で消し去りたいオブジェクトの名前リスト")]
    public List<string> targetNames = new List<string>();

    private int currentExplosionDamage = 1;

    public void SetDamage(int damage)
    {
        currentExplosionDamage = damage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        string hitObjectName = other.gameObject.name;

        // ★修正点：当たったオブジェクト自身、親、子、どこにBombがあっても見つけ出す
        Bomb targetBomb = other.gameObject.GetComponent<Bomb>();
        if (targetBomb == null) targetBomb = other.gameObject.GetComponentInChildren<Bomb>();
        if (targetBomb == null) targetBomb = other.gameObject.GetComponentInParent<Bomb>();

        if (targetBomb != null)
        {
            // プレイヤー配置フラグに関係なく強制爆発
            Debug.Log($"【誘爆発生】爆風が爆弾にヒットしました: {hitObjectName}");
            targetBomb.Explode();
            return; // 爆弾の処理をしたのでここで終了
        }

        // 以下は通常のブロック消去処理
        foreach (string targetName in targetNames)
        {
            if (string.IsNullOrEmpty(targetName)) continue;

            if (hitObjectName.StartsWith(targetName))
            {
                FallObject fallObj = other.gameObject.GetComponent<FallObject>();
                if (fallObj != null)
                {
                    if (fallObj.isFall) continue;

                    fallObj.TakeDamage(currentExplosionDamage);
                    fallObj.UpdateSprite();
                    Debug.Log($"爆風ヒット: {hitObjectName} に {currentExplosionDamage} ダメージを与えました。");
                }
                else
                {
                    Destroy(other.gameObject);
                }
                break;
            }
        }
    }
}