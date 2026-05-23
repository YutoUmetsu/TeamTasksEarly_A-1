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

    // 1個設置した後に次の入力を受け付けないためのロック用フラグ
    private bool isLocked = false;

    void Start()
    {
        RefreshStockUI();
    }

    void Update()
    {
        // ロックされている場合はクリック判定自体を行わない
        if (isLocked) return;

        if (Input.GetMouseButtonDown(0))
        {
            CheckAndPlace();
        }
    }

    public void RefreshStockUI()
    {
        if (uiStockContainer == null || uiIconPrefab == null) return;

        foreach (Transform child in uiStockContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < stockItems.Count; i++)
        {
            GameObject iconObj = Instantiate(uiIconPrefab, uiStockContainer);
            Bomb bombScript = stockItems[i].GetComponent<Bomb>();

            if (bombScript != null && bombScript.uiSprite != null)
            {
                UnityEngine.UI.Image iconImage = iconObj.GetComponent<UnityEngine.UI.Image>();

                if (iconImage != null)
                {
                    iconImage.sprite = bombScript.uiSprite;
                }
            }
        }
    }

    void CheckAndPlace()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObj = hit.collider.gameObject;

            if (targetItems.Contains(clickedObj))
            {
                Bomb existingBomb = clickedObj.GetComponentInChildren<Bomb>();
                if (existingBomb != null)
                {
                    Debug.Log("ここにはすでに爆弾が設置されています！");
                    return;
                }

                if (stockItems.Count > 0 && totalPlacedCount < maxPlaceableBombs)
                {
                    PlaceItemFromStock(clickedObj);
                }
                else
                {
                    FallObject fallObj = clickedObj.GetComponent<FallObject>();
                    Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();

                    if (fallObj != null && controller != null)
                    {
                        controller.PerformBlockDelete(fallObj);
                    }
                }
            }
        }
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
        RefreshStockUI();
        totalPlacedCount++;

        // 1個置いたので即座に設置システムをロックする
        isLocked = true;

        if (makeSwitch != null)
        {
            makeSwitch.BombCount++;
        }

        Debug.Log($"設置完了！ {totalPlacedCount}/{maxPlaceableBombs}個。次の起爆まで設置をロックします。");
    }

    public void RegisterTarget(GameObject target)
    {
        if (!targetItems.Contains(target))
        {
            targetItems.Add(target);
        }
    }

    // スイッチ側から呼び出して、次の設置を行えるようにロックを外すための関数
    public void UnlockPlacement()
    {
        isLocked = false;
        Debug.Log("爆弾の設置ロックを解除しました。");
    }
}