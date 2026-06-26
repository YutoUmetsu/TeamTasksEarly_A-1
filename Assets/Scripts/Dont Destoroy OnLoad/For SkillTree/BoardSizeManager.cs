using UnityEngine;

public class BoardSizeManager : MonoBehaviour
{
    public static BoardSizeManager Instance { get; private set; }

    [Header("スキルで拡張された盤面の追加サイズ（正方形のまま増える）")]
    public int bonusBoardSize = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // スキルを取得した時にサイズを増やす関数
    public void IncreaseBoardSize(int amount)
    {
        bonusBoardSize += amount;
        Debug.Log($"盤面サイズボーナスが +{amount} されました！ 現在の追加サイズ: {bonusBoardSize}");
    }
}