using UnityEngine; 

public class ExplosionHit : MonoBehaviour
{
    // トリガー判定（Is Triggerにチェックが入っている場合）
    void OnTriggerEnter2D(Collider2D other)
    {
        // 当たった相手の親、または自分自身に Bomb スクリプトがついているか確認
        Bomb otherBomb = other.GetComponentInParent<Bomb>();

        if (otherBomb != null)
        {
            // まだ爆発していない場合だけ起爆（無限ループ防止）
            otherBomb.Explode();
        }
    }
}
