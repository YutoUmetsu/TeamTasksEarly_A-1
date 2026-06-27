using UnityEngine;
using System.Collections.Generic;

public class BombSet : MonoBehaviour
{
    [Header("設置する爆弾のストック (最大5個など)")]
    public List<GameObject> stockItems = new List<GameObject>();

    [Header("クリック対象のリスト（2と3を入れる）")]
    public List<GameObject> targetItems = new List<GameObject>();

    public MakeSwitch makeSwitch;

    [Header("このゲーム中に設置できる爆弾の最大合計数（最初は3）")]
    public int maxPlaceableBombs = 3;

    private int totalPlacedCount = 0;

    [Header("UI：Vertical Layout Groupをつけた親オブジェクト")]
    [SerializeField] private Transform uiStockContainer;

    [Header("UI：縦に並べたい爆弾アイコンのプレハブ")]
    [SerializeField] private GameObject uiIconPrefab;

    // ─── ★追加：説明用UIオブジェクトの参照 ───
    [Header("説明用UI：1つ目のセット（初期状態・起爆後用）")]
    [SerializeField] private GameObject explanationUI_First;

    [Header("説明用UI：2つ目のセット（爆弾設置後用）")]
    [SerializeField] private GameObject explanationUI_Second;

    private bool isLocked = false;

    // ★修正点：現在ドラッグ中のハンドラーを直接参照として保持する
    public BombUIDragHandler CurrentlyDraggingHandler { get; set; }

    void Start()
    {
        RefreshStockUI();

        // ★追加：ゲーム開始時は1つ目の説明だけを表示し、2つ目は非表示にする
        UpdateExplanationVisual(isBombPlaced: false);
    }

    public void RefreshStockUI()
    {
        if (uiStockContainer == null || uiIconPrefab == null) return;

        // 1. 古いUIアイコンを削除
        List<Transform> childrenToRemove = new List<Transform>();
        foreach (Transform child in uiStockContainer)
        {
            BombUIDragHandler dragHandler = child.GetComponent<BombUIDragHandler>();
            if (dragHandler != null && dragHandler.IsCurrentlyDragging)
            {
                continue; // ドラッグ中のものは削除しない
            }
            childrenToRemove.Add(child);
        }

        foreach (Transform child in childrenToRemove)
        {
            child.SetParent(null);
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        // 2. 残り設置可能数の計算
        int currentMaxBombs = maxPlaceableBombs;
        if (BombInventoryManager.Instance != null)
        {
            currentMaxBombs += BombInventoryManager.Instance.bonusMaxBombs;
        }

        int remainingCount = currentMaxBombs - totalPlacedCount;
        if (remainingCount < 0) remainingCount = 0;

        int displayCount = Mathf.Min(stockItems.Count, remainingCount);

        // 3. ドラッグ状態のチェック
        bool isDraggingNow = (CurrentlyDraggingHandler != null && CurrentlyDraggingHandler.IsCurrentlyDragging);
        int startIndex = isDraggingNow ? 1 : 0;

        // 4. UIアイコンの生成ループ
        for (int i = startIndex; i < displayCount; i++)
        {
            GameObject iconObj = Instantiate(uiIconPrefab, uiStockContainer);
            Bomb bombScript = stockItems[i].GetComponent<Bomb>();

            // ★修正：ドラッグ中（startIndex=1）の時は、本来のデータ位置「i」をそのまま見た目のインデックスとして使う
            // これにより、ドラッグ中も残った次の爆弾は「1番目（待機）」として正しく計算されます
            int uiVisualIndex = isDraggingNow ? i : (i - startIndex);

            if (bombScript != null)
            {
                UnityEngine.UI.Image iconImage = iconObj.GetComponent<UnityEngine.UI.Image>();
                if (iconImage != null)
                {
                    // uiVisualIndexが0のときだけアクティブ画像（導火線あり）になる
                    if (uiVisualIndex == 0)
                    {
                        iconImage.sprite = bombScript.GetCurrentUiSprite();
                    }
                    else
                    {
                        iconImage.sprite = bombScript.GetCurrentWaitingUiSprite();
                    }
                }
            }

            BombUIDragHandler dragHandler = iconObj.GetComponent<BombUIDragHandler>();
            if (dragHandler == null) dragHandler = iconObj.AddComponent<BombUIDragHandler>();

            // 盤面にすでに爆弾があるかチェック
            bool hasAnyPlacedBomb = false;
            foreach (GameObject block in targetItems)
            {
                if (block == null) continue;
                Bomb[] bombs = block.GetComponentsInChildren<Bomb>();
                foreach (Bomb b in bombs)
                {
                    if (b.gameObject != block)
                    {
                        hasAnyPlacedBomb = true;
                        break;
                    }
                }
                if (hasAnyPlacedBomb) break;
            }

            // ドラッグ可能判定も uiVisualIndex を基準にする
            bool canDragNow = (uiVisualIndex == 0) && (!isLocked) && (!hasAnyPlacedBomb);
            dragHandler.Setup(this, canDragNow);
        }
    }

    public bool TryPlaceFromDrag(Vector2 screenMousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenMousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObj = hit.collider.gameObject;

            if (targetItems.Contains(clickedObj))
            {
                bool isBombBlock = clickedObj.GetComponent<Bomb>() != null;

                Bomb alreadyPlacedBomb = null;
                foreach (GameObject block in targetItems)
                {
                    if (block == null) continue;
                    Bomb[] bombs = block.GetComponentsInChildren<Bomb>();
                    foreach (Bomb b in bombs)
                    {
                        if (b.gameObject != block)
                        {
                            alreadyPlacedBomb = b;
                            break;
                        }
                    }
                    if (alreadyPlacedBomb != null) break;
                }

                if (alreadyPlacedBomb != null && alreadyPlacedBomb.transform.parent == clickedObj.transform)
                {
                    return false;
                }

                if (isBombBlock)
                {
                    Debug.Log("ここには設置できません！");
                    return false;
                }

                int currentMaxBombs = maxPlaceableBombs;
                if (BombInventoryManager.Instance != null)
                {
                    currentMaxBombs += BombInventoryManager.Instance.bonusMaxBombs;
                }

                if (alreadyPlacedBomb != null)
                {
                    alreadyPlacedBomb.transform.position = clickedObj.transform.position;
                    alreadyPlacedBomb.transform.SetParent(clickedObj.transform);

                    alreadyPlacedBomb.isPlayerPlaced = true;

                    Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
                    if (controller != null)
                    {
                        controller.isBombPlacedThisFrame = true;
                    }

                    NotifySwitchUpdate();
                    RefreshStockUI();

                    // ★追加：置き直し時も「すでに設置されている状態」なので2つ目を維持
                    UpdateExplanationVisual(isBombPlaced: true);

                    Debug.Log("爆弾を別の場所に置き直しました！");
                    return true;
                }
                else if (stockItems.Count > 0 && totalPlacedCount < currentMaxBombs)
                {
                    // 新規設置
                    PlaceItemFromStock(clickedObj);
                    return true;
                }
            }
        }
        return false;
    }

    void PlaceItemFromStock(GameObject targetBlock)
    {
        GameObject itemToPlace = stockItems[0];

        GameObject spawnedBomb = Instantiate(itemToPlace, targetBlock.transform.position, Quaternion.identity);
        spawnedBomb.transform.SetParent(targetBlock.transform);

        Bomb bombComp = spawnedBomb.GetComponent<Bomb>();
        if (bombComp != null)
        {
            bombComp.isPlayerPlaced = true;
        }

        Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
        if (controller != null)
        {
            controller.isBombPlacedThisFrame = true;
        }

        stockItems.RemoveAt(0);
        totalPlacedCount++;

        RefreshStockUI();

        if (makeSwitch != null)
        {
            makeSwitch.BombCount++;
        }

        // ★追加：爆弾が新規設置されたので、説明UIを「2つ目」に切り替える
        UpdateExplanationVisual(isBombPlaced: true);

        NotifySwitchUpdate();
        Debug.Log($"設置完了！起爆ボタンを押すまで置き直しが可能です。");
    }

    public bool IsPlacementLocked()
    {
        return isLocked;
    }

    public void RegisterTarget(GameObject target)
    {
        if (!targetItems.Contains(target))
        {
            targetItems.Add(target);
        }
    }

    public void UnlockPlacement()
    {
        isLocked = false;
        RefreshStockUI();
        Debug.Log("爆弾の設置ロックを解除しました。");
    }

    public void OnExplodeResetUI()
    {
        RefreshStockUI();
        NotifySwitchUpdate();

        // ★追加：起爆されて盤面がリセットされたので、説明UIを「1つ目」に戻す
        UpdateExplanationVisual(isBombPlaced: false);
    }

    private void NotifySwitchUpdate()
    {
        ExplosionSwitchVisualManager visualManager = UnityEngine.Object.FindFirstObjectByType<ExplosionSwitchVisualManager>();
        if (visualManager != null)
        {
            visualManager.UpdateSwitchVisual(stockItems.Count);
        }
    }

    // ★追加：説明用UIセットのアクティブ状態を切り替えるヘルパー関数
    private void UpdateExplanationVisual(bool isBombPlaced)
    {
        if (explanationUI_First != null)
        {
            explanationUI_First.SetActive(!isBombPlaced);
        }
        if (explanationUI_Second != null)
        {
            explanationUI_Second.SetActive(isBombPlaced);
        }
    }

    public int TotalPlacedCount => totalPlacedCount;
}