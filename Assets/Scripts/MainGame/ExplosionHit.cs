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
                FallObject fallObj = other.gameObject.GetComponent<FallObject>();
                if (fallObj != null)
                {
                    // 【重要】もしすでに落下中のブロック（上から降ってきた無実のブロック）なら、爆風の処理を無視する！
                    if (fallObj.isFall) continue;

                    Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
                    if (controller != null)
                    {
                        // 将来ここを「fallObj.TakeDamage(1);」などのHP減少処理に差し替えることも可能です！
                        controller.SpawnCoinImmediate(fallObj);
                        fallObj.SetDelete();
                    }
                    else
                    {
                        fallObj.SetDelete();
                    }

                    Debug.Log($"爆風ヒット: {hitObjectName} を安全に消去予定にしました。");
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