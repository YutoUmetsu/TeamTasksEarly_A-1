using UnityEngine;

public class MakeSwitch : MonoBehaviour
{
    public int BombCount { get; set; }

    [Header("スイッチの見た目オブジェクトをここに")]
    public GameObject bombGeneratorObj;

    void Start()
    {
        // 見た目だけを隠す（このスクリプト自体は動いたまま）
        if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);
    }

    void Update()
    {
        // 5個置かれたら見た目を表示
        if (BombCount >= 5 && bombGeneratorObj != null && !bombGeneratorObj.activeSelf)
        {
            bombGeneratorObj.SetActive(true);
        }
    }

    public void ExplodeNearestBomb()
    {
        // FindObjectsByType に修正した（警告対策）
        Bomb[] allBombs = Object.FindObjectsByType<Bomb>(FindObjectsSortMode.None);

        Bomb nearestBomb = null;
        float minDistance = float.MaxValue;

        // MakeSwitch.cs の中
        foreach (Bomb b in allBombs)
        {
            // Vector3をVector2として扱う（Zを無視する）
            Vector2 myPos = transform.position;
            Vector2 bombPos = b.transform.position;

            float distance = Vector2.Distance(myPos, bombPos);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestBomb = b;
            }
        }


        if (nearestBomb != null)
        {
            nearestBomb.Explode();

            // 起爆したら「見た目」だけをまた隠す
            if (bombGeneratorObj != null) bombGeneratorObj.SetActive(false);

            // カウントをリセット（また5個置いたら出したい場合）
            BombCount = 0;
        }
    }
}
