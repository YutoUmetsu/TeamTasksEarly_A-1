using UnityEngine;
using System.Collections.Generic;

public class ExplosionHit : MonoBehaviour
{
    [Header("爆風で消し去りたいオブジェクトの名前リスト")]
    public List<string> targetNames = new List<string>();

    // 爆弾から引き継ぐ今回のダメージ値
    private int currentExplosionDamage = 1;

    /// <summary>
    /// 爆弾スクリプトから攻撃力を受け取るための関数
    /// </summary>
    public void SetDamage(int damage)
    {
        currentExplosionDamage = damage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string hitObjectName = other.gameObject.name;

        foreach (string targetName in targetNames)
        {
            if (string.IsNullOrEmpty(targetName)) continue;

            if (hitObjectName.StartsWith(targetName))
            {
                FallObject fallObj = other.gameObject.GetComponent<FallObject>();
                if (fallObj != null)
                {
                    // もしすでに落下中のブロック（上から降ってきた無実のブロック）なら、爆風の処理を無視する！
                    if (fallObj.isFall) continue;

                    Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
                    if (controller != null)
                    {
                        // ─── 【ここをきっちり修正！】 ───
                        // コントローラーがいてもいなくても、確実にブロックへダメージを与える
                        fallObj.TakeDamage(currentExplosionDamage);
                    }
                    else
                    {
                        fallObj.TakeDamage(currentExplosionDamage);
                    }

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