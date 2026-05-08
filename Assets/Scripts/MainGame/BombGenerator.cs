using UnityEngine;

public class BombGenerator : MonoBehaviour
{
    // 全体の爆破処理をここに書く
    public void ExplodeAll()
    {
        Debug.Log("ドカーン！全ての爆弾を爆破しました！");

        // 爆破後の後片付け
        this.gameObject.SetActive(false); // スイッチを隠す
    }
}
