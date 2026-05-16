using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("判定まとめ用空オブジェクトをセット")]
    public GameObject explosionAreaGroup;

    [Header("爆発演出の持続時間")]
    public float destroyDelay = 0.5f;

    [Header("この爆弾がUIのストックに並ぶ時の画像")]
    public Sprite uiSprite;

    void Start()
    {
        // ゲーム開始時は爆発判定を隠しておく
        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(false);
        }
    }

    // スイッチから呼ばれる関数
    // スイッチから呼ばれる関数
    public void Explode()
    {
        Debug.Log("ドカーン！起爆しました");

       
        // 爆発する前に、自分が乗っている親（ブロック）のスクリプトを取得
        BlockFall parentBlock = GetComponentInParent<BlockFall>();

        if (parentBlock != null)
        {
            // 親ブロックのデータを強制的に「deleted」にする
            BlockSpawn.blockInfo[parentBlock.myX, parentBlock.myY].blocks = Blocks.deleted;
            Debug.Log($"足元のブロック ({parentBlock.myX}, {parentBlock.myY}) を道連れにします！");
        }
      
        // 足元をdeletedにした後、親子関係を解除して独立させる
        transform.SetParent(null);


        // 1. 爆発判定（周り用）を有効化する
        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(true);
        }

        // 2. 爆弾本体（見た目）を隠す処理
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        // 3. 少し待ってから爆弾（判定）を削除
        Destroy(gameObject, destroyDelay);
    }
}
