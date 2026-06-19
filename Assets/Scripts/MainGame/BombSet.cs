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

    private bool isLocked = false;

    void Start()
    {
        RefreshStockUI();
    }

    public void RefreshStockUI()
    {
        if (uiStockContainer == null || uiIconPrefab == null) return;

        foreach (Transform child in uiStockContainer)
        {
            Destroy(child.gameObject);
        }

        int currentMaxBombs = maxPlaceableBombs;
        if (BombInventoryManager.Instance != null)
        {
            currentMaxBombs += BombInventoryManager.Instance.bonusMaxBombs;
        }

        int remainingCount = currentMaxBombs - totalPlacedCount;
        if (remainingCount < 0) remainingCount = 0;

        // ★お友達の元の正しい計算に戻しました
        int displayCount = Mathf.Min(stockItems.Count, remainingCount);

        for (int i = 0; i < displayCount; i++)
        {
            GameObject iconObj = Instantiate(uiIconPrefab, uiStockContainer);
            Bomb bombScript = stockItems[i].GetComponent<Bomb>();

            if (bombScript != null && bombScript.uiSprite != null)
            {
                UnityEngine.UI.Image iconImage = iconObj.GetComponent<UnityEngine.UI.Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = bombScript.GetCurrentUiSprite();
                }
            }

            BombUIDragHandler dragHandler = iconObj.GetComponent<BombUIDragHandler>();
            if (dragHandler == null) dragHandler = iconObj.AddComponent<BombUIDragHandler>();

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

            bool canDragNow = (i == 0) && (!isLocked) && (!hasAnyPlacedBomb);
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

                    Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
                    if (controller != null)
                    {
                        controller.isBombPlacedThisFrame = true;
                    }

                    // スイッチの見た目を更新
                    NotifySwitchUpdate();

                    Debug.Log("爆弾を別の場所に置き直しました！");
                    return true;
                }
                else if (stockItems.Count > 0 && totalPlacedCount < currentMaxBombs)
                {
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

        // スイッチの見た目を更新
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
        // ★余計なリセット処理をすべて削除し、お友達の元のコードに完全復元しました
        RefreshStockUI();
        NotifySwitchUpdate();
    }

    // スイッチへ通知を送る安全な共通関数
    private void NotifySwitchUpdate()
    {
        ExplosionSwitchVisualManager visualManager = UnityEngine.Object.FindFirstObjectByType<ExplosionSwitchVisualManager>();
        if (visualManager != null)
        {
            visualManager.UpdateSwitchVisual(stockItems.Count); // 今の純粋な手札の数を送る
        }
    }
    public int TotalPlacedCount => totalPlacedCount;
}