using UnityEngine;
using System.Collections.Generic;

public class ExplosionHit : MonoBehaviour
{
    [Header("爆風で消し去りたいオブジェクトの名前リスト（プレファブ名）")]
    public List<string> targetNames = new List<string>();

    // 自分の爆風（コライダー）に、何か他のオブジェクトが触れた瞬間に動く
    void OnTriggerEnter2D(Collider2D other)
    {
        // 当たった相手の名前を取得
        string hitObjectName = other.gameObject.name;

        // リストに登録された名前を1つずつチェックする
        foreach (string targetName in targetNames)
        {
            // 名前が空っぽなら飛ばす
            if (string.IsNullOrEmpty(targetName)) continue;

            // ─── ここがポイント！ ───
            // 当たった相手の名前が、登録した名前から始まっているかチェック
            // これにより「Enemy」も「Enemy(Clone)」も両方捕まえることができます
            if (hitObjectName.StartsWith(targetName))
            {
                Debug.Log($"爆風がヒット！ {hitObjectName} を消去します。");

                // 巻き込まれたオブジェクトを削除する
                Destroy(other.gameObject);

                // 1つ合致したらこのオブジェクトに対するチェックは終了
                break;
            }
        }
    }
}