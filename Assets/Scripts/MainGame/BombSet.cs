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
        if (stockItems.Count <= 0)
        {
            Debug.Log("ストックが空です！");
            return;
        }

        if (totalPlacedCount >= maxPlaceableBombs)
        {
            Debug.Log($"これ以上置けません！ 上限: {maxPlaceableBombs}個");
            return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObj = hit.collider.gameObject;

            if (targetItems.Contains(clickedObj))
            {
                PlaceItemFromStock(clickedObj.transform.position);
            }
        }
    }

    void PlaceItemFromStock(Vector3 position)
    {
        GameObject itemToPlace = stockItems[0];
        Instantiate(itemToPlace, position, Quaternion.identity);
        stockItems.RemoveAt(0);

        // ─── 修正：消費した時も、最新状態にUIをリフレッシュするだけ！ ───
        RefreshStockUI();

        totalPlacedCount++;

        if (makeSwitch != null)
        {
            makeSwitch.BombCount++;
        }

        Debug.Log($"設置完了！ 累計設置: {totalPlacedCount}/{maxPlaceableBombs}個");
    }
}