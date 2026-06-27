using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BombUIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private int originalIndex;
    private BombSet bombSet;
    private bool isFirstIcon = false;

    public bool IsCurrentlyDragging { get; private set; } = false;

    public void Setup(BombSet set, bool canDrag)
    {
        bombSet = set;
        isFirstIcon = canDrag;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (!isFirstIcon)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isFirstIcon || bombSet == null || bombSet.IsPlacementLocked()) return;

        IsCurrentlyDragging = true;
        bombSet.CurrentlyDraggingHandler = this; // ★修正点：BombSetに「私が今ドラッグ中だよ」と直接教える

        originalPosition = transform.position;
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();

        transform.SetParent(originalParent.root);
        canvasGroup.blocksRaycasts = false;

        // ★追加：ドラッグ開始直後に周りのUI（待機画像など）を即座に正しく整列させる
        bombSet.RefreshStockUI();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isFirstIcon || bombSet == null || bombSet.IsPlacementLocked()) return;

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isFirstIcon || bombSet == null || bombSet.IsPlacementLocked()) return;

        IsCurrentlyDragging = false;

        // 判定を取る前に一度解除
        if (bombSet.CurrentlyDraggingHandler == this)
        {
            bombSet.CurrentlyDraggingHandler = null;
        }

        bool success = bombSet.TryPlaceFromDrag(eventData.position);

        if (success)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
            transform.position = originalPosition;
            canvasGroup.blocksRaycasts = true;

            bombSet.RefreshStockUI();
        }
    }
}