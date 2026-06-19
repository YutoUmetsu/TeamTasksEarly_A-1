using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Unityのドラッグ機能（EventSystems）をフル活用する呪文
public class BombUIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private BombSet bombSet;
    private bool isFirstIcon = false;

    // 一番上のアイコンだけドラッグできるようにBombSetから初期化してもらう関数
    public void Setup(BombSet set, bool canDrag)
    {
        bombSet = set;
        isFirstIcon = canDrag;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 一番上じゃないアイコンは半透明にして、掴めないようにレイキャストを無視させる
        if (!isFirstIcon)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    // ドラッグを開始した瞬間
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isFirstIcon || bombSet == null || bombSet.IsPlacementLocked()) return;

        originalPosition = transform.position;
        originalParent = transform.parent;

        // ドラッグ中のアイコンが他のUIの背後に隠れないように、一時的にルート（最前面）に出す
        transform.SetParent(originalParent.root);

        // ドラッグ中のマウスの真下にある「ゲーム画面（ブロック）」を検知できるように、UIの当たり判定を一時OFFにする
        canvasGroup.blocksRaycasts = false;
    }

    // ドラッグ（マウス移動）中
    public void OnDrag(PointerEventData eventData)
    {
        if (!isFirstIcon || bombSet == null || bombSet.IsPlacementLocked()) return;

        // マウスの位置にUIアイコンをぴったり追従させる
        transform.position = eventData.position;
    }

    // マウスを離した（ドロップした）瞬間
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isFirstIcon || bombSet == null || bombSet.IsPlacementLocked()) return;

        // ドロップした位置からゲーム画面に向かってレイキャストを飛ばし、設置を試みる
        bool success = bombSet.TryPlaceFromDrag(eventData.position);

        if (success)
        {
            // 設置成功したら、このドラッグ用UIは役目を終えたので消滅
            Destroy(gameObject);
        }
        else
        {
            // 設置失敗（画面外や置けない場所）なら、元のUIの並び（Vertical Layout Group）に戻す
            transform.SetParent(originalParent);
            transform.position = originalPosition;
            canvasGroup.blocksRaycasts = true;
        }
    }
}