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
                    //修正：爆風が当たった「その瞬間」に、その場で即座にコインを出す！
                    Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
                    if (controller != null)
                    {
                        controller.SpawnCoinImmediate(fallObj);
                    }

                    fallObj.SetDelete();
                    Debug.Log($"爆風ヒット: {hitObjectName} を消去予定にしました。");
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