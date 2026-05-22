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

    void Start()
    {
        // ─── 修正：最初は現在のストックでUIを作る ───
        RefreshStockUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckAndPlace();
        }
    }

    // UIとストックの中身を連動・最新化
    public void RefreshStockUI()
    {
        if (uiStockContainer == null || uiIconPrefab == null) return;

        // 1. 古いUIアイコンを全部削除
        foreach (Transform child in uiStockContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 現在の最新ストックの分だけUIを並べ直す
        for (int i = 0; i < stockItems.Count; i++)
        {
            // UIの土台を生成
            GameObject iconObj = Instantiate(uiIconPrefab, uiStockContainer);

            // ─── ここが追加：爆弾の種類に合わせて画像を変える ───
            // ストックにある爆弾のプレファブから、Bombスクリプトを取得
            Bomb bombScript = stockItems[i].GetComponent<Bomb>();

            if (bombScript != null && bombScript.uiSprite != null)
            {
                // UIプレハブについている UnityEngine.UI.Image コンポーネントを探す
                UnityEngine.UI.Image iconImage = iconObj.GetComponent<UnityEngine.UI.Image>();

                if (iconImage != null)
                {
                    // 爆弾が持っている専用の画像に差し替える！
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

            // クリックしたものが、登録されているパズルブロックリストに含まれている場合
            if (targetItems.Contains(clickedObj))
            {
                // すでに爆弾が置いてあるかチェック
                Bomb existingBomb = clickedObj.GetComponentInChildren<Bomb>();
                if (existingBomb != null)
                {
                    Debug.Log("ここにはすでに爆弾が設置されています！");
                    return;
                }



                // もし爆弾のストックがあり、かつ設置上限にも達していないなら「爆弾を設置」
                if (stockItems.Count > 0 && totalPlacedCount < maxPlaceableBombs)
                {
                    PlaceItemFromStock(clickedObj);
                }
                // ストックがない、または上限に達している場合は「通常のブロック消去」として処理する！
                //(ここはあくまでデバッグ用、後で必ず処理を消すこと♭)
                else
                {
                    FallObject fallObj = clickedObj.GetComponent<FallObject>();
                    Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();

                    if (fallObj != null && controller != null)
                    {
                        // Controllerに「この通常ブロックを消して、落下させて」とバトンを渡す
                        controller.PerformBlockDelete(fallObj);
                    }
                }
            }
        }
    }

    // 受け取り側もちゃんと GameObject になっているか確認
    void PlaceItemFromStock(GameObject targetBlock)
    {
        GameObject itemToPlace = stockItems[0];

        // ブロックと同じ位置に、ブロックの子として生成
        GameObject spawnedBomb = Instantiate(itemToPlace, targetBlock.transform.position, Quaternion.identity);
        spawnedBomb.transform.SetParent(targetBlock.transform);

        // コントローラーに「爆弾置いたから消すな」と通知
        Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
        if (controller != null)
        {
            controller.isBombPlacedThisFrame = true;
        }
       
        stockItems.RemoveAt(0);
        RefreshStockUI();
        totalPlacedCount++;

        if (makeSwitch != null)
        {
            makeSwitch.BombCount++;
        }

        Debug.Log($"設置完了！ {totalPlacedCount}/{maxPlaceableBombs}個");
    }

    // ─── 追加：生成されたブロックをターゲットリストに入れる関数 ───
    public void RegisterTarget(GameObject target)
    {
        if (!targetItems.Contains(target))
        {
            targetItems.Add(target);
        }
    }
}