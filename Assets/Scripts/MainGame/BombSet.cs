using UnityEngine;
using System.Collections.Generic;

public class BombSet : MonoBehaviour
{
    [Header("設置する爆弾のストック (5個)")]
    public List<GameObject> stockItems = new List<GameObject>();

    [Header("クリック対象のリスト（2と3を入れる）")]
    public List<GameObject> targetItems = new List<GameObject>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckAndPlace();
        }
    }

    void CheckAndPlace()
    {
        // 1. ストックが空なら何もしない
        if (stockItems.Count <= 0)
        {
            Debug.Log("ストックが空です！");
            return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObj = hit.collider.gameObject;

            // 2. クリックしたものがターゲットに含まれているか
            if (targetItems.Contains(clickedObj))
            {
                // リストの先頭（0番目）にあるアイテムを設置
                PlaceItemFromStock(clickedObj.transform.position);
            }
        }
    }

    void PlaceItemFromStock(Vector3 position)
    {
        // 先頭のアイテムを取得
        GameObject itemToPlace = stockItems[0];

        // 設置
        Instantiate(itemToPlace, position, Quaternion.identity);

        // リストから削除（これで次のアイテムが0番目になる）
        stockItems.RemoveAt(0);

        Debug.Log($"設置完了！ 残りストック: {stockItems.Count}個");
    }
}
