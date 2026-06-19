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

            // 生成したUIにドラッグ管理コンポーネントをつけて初期化する
            BombUIDragHandler dragHandler = iconObj.GetComponent<BombUIDragHandler>();
            if (dragHandler == null) dragHandler = iconObj.AddComponent<BombUIDragHandler>();

            // 画面上にすでに手動設置された爆弾があるかチェック
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

            // 「リストの一番上（i == 0）」かつ「今設置ロックがかかっていない（!isLocked）」
            // かつ「まだ画面に爆弾を1個も置いていない」時だけ新しくドラッグ可能にする！
            bool canDragNow = (i == 0) && (!isLocked) && (!hasAnyPlacedBomb);

            dragHandler.Setup(this, canDragNow);
        }
    }

    //ドラッグ＆ドロップでの設置・置き直し処理（完全修正版）
    public bool TryPlaceFromDrag(Vector2 screenMousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenMousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObj = hit.collider.gameObject;

            if (targetItems.Contains(clickedObj))
            {
                // 1. そのブロックにすでに「他の爆弾（最初から降ってきたやつ等）」がないか
                bool isBombBlock = clickedObj.GetComponent<Bomb>() != null;

                // 2. 画面上にすでに「プレイヤーが手動で置いた爆弾」があるか探す
                Bomb alreadyPlacedBomb = null;
                foreach (GameObject block in targetItems)
                {
                    if (block == null) continue;
                    Bomb[] bombs = block.GetComponentsInChildren<Bomb>();
                    foreach (Bomb b in bombs)
                    {
                        // ブロック自身ではなく、子供としてくっついている ＝ 手動で置いた爆弾
                        if (b.gameObject != block)
                        {
                            alreadyPlacedBomb = b;
                            break;
                        }
                    }
                    if (alreadyPlacedBomb != null) break;
                }

                // 今ドロップした場所に、すでに自分が置いた爆弾があるならスルー
                if (alreadyPlacedBomb != null && alreadyPlacedBomb.transform.parent == clickedObj.transform)
                {
                    return false;
                }

                // 元々の爆弾ブロックの上には置けないガード
                if (isBombBlock)
                {
                    Debug.Log("ここには設置できません！");
                    return false;
                }

                // 枠数チェック
                int currentMaxBombs = maxPlaceableBombs;
                if (BombInventoryManager.Instance != null)
                {
                    currentMaxBombs += BombInventoryManager.Instance.bonusMaxBombs;
                }

                // ─── 設置、または置き直しの実行 ───
                if (alreadyPlacedBomb != null)
                {
                    // すでに置いてある爆弾を、新しいブロックに引っ越し
                    alreadyPlacedBomb.transform.position = clickedObj.transform.position;
                    alreadyPlacedBomb.transform.SetParent(clickedObj.transform);

                    Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
                    if (controller != null)
                    {
                        controller.isBombPlacedThisFrame = true;
                    }

                    Debug.Log("爆弾を別の場所に置き直しました！");
                    return true;
                }
                else if (stockItems.Count > 0 && totalPlacedCount < currentMaxBombs)
                {
                    // まだ画面に1個も置いてない場合
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

        // リストから削除してUIを詰める
        stockItems.RemoveAt(0);
        totalPlacedCount++;

        // 置いた時点ではまだロックをかけない
        RefreshStockUI();

        if (makeSwitch != null)
        {
            makeSwitch.BombCount++;
        }

        Debug.Log($"設置完了！起爆ボタンを押すまで置き直しが可能です。");
    }

    // ロック状態を外から安全にチェックするための公開関数
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
        RefreshStockUI(); // ロック解除された時にも念のためUIを再描画して一番上を掴めるようにする
        Debug.Log("爆弾の設置ロックを解除しました。");
    }

    public void OnExplodeResetUI()
    {
        RefreshStockUI();
    }
}