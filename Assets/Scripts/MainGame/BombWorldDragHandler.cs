using UnityEngine;

public class BombWorldDragHandler : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Transform originalParent;
    private Vector3 originalPosition;
    private BombSet bombSet;
    private Collider2D myCollider;

    void Start()
    {
        bombSet = UnityEngine.Object.FindFirstObjectByType<BombSet>();
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // もし起爆ロック中なら何もしない
        if (bombSet != null && bombSet.IsPlacementLocked()) return;

        //  マウスが押された瞬間（掴む判定）
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // マウスの位置に「自分自身のコライダー」があるか直接チェックする！
            // これで他のオブジェクトに隠れていても、自分をピンポイントで掴めます
            if (myCollider != null && myCollider.OverlapPoint(mousePos))
            {
                isDragging = true;
                originalPosition = transform.position;
                originalParent = transform.parent;

                // 掴んだ瞬間のズレ（オフセット）を計算
                offset = transform.position - mousePos;

                // ドラッグ中に親ブロックと一緒に動かないように親子関係を一時解除
                transform.SetParent(null);
                Debug.Log("爆弾を直接掴みました！");
            }
        }

        //  ドラッグ中（マウスが押され続けている間）
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos + offset;
        }

        //  マウスを離した瞬間（ドロップ判定）
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

           
            // 自分のコライダーを一時的に無効化して、裏側のブロックが見えるようにする
            if (myCollider != null) myCollider.enabled = false;

            // 爆弾の中心位置にあるブロックを探す（自分のコライダーはOFFなので奥まで届く！）
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);

            // レイキャストが終わったら、自分のコライダーをすぐ元に戻す
            if (myCollider != null) myCollider.enabled = true;

            if (hit.collider != null && bombSet != null)
            {
                GameObject targetBlock = hit.collider.gameObject;

                // 移動先のブロックが対象（2や3）かチェック
                if (bombSet.targetItems.Contains(targetBlock))
                {
                    bool isBombBlock = targetBlock.GetComponent<Bomb>() != null;
                    Bomb[] allBombs = targetBlock.GetComponentsInChildren<Bomb>();
                    bool hasPlacedBomb = false;

                    foreach (Bomb b in allBombs)
                    {
                        if (b.gameObject != targetBlock)
                        {
                            hasPlacedBomb = true;
                            break;
                        }
                    }

                    // 他の爆弾がなければ引っ越し成功！
                    if (!isBombBlock && !hasPlacedBomb)
                    {
                        transform.position = targetBlock.transform.position;
                        transform.SetParent(targetBlock.transform);
                        Debug.Log("爆弾の置き直しに成功しました！");
                        return;
                    }
                }
            }

            // 置けない場所なら元のブロックに戻す
            transform.SetParent(originalParent);
            transform.position = originalPosition;
            Debug.Log("置けない場所なので元の位置に戻しました。");
        }
    }
}