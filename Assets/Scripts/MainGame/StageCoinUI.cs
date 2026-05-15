using UnityEngine;
using TMPro;

public class StageCoinUI : MonoBehaviour
{
    // ステージのヒエラルキー上にあるTMPをインスペクターで繋ぐ
    [SerializeField] private TextMeshProUGUI stageTotalCoinText;

    void Update()
    {
        // 常にCoinManagerの総量を覗き見して、リアルタイムに表示を更新する
        if (stageTotalCoinText != null && CoinManager.Instance != null)
        {
            stageTotalCoinText.text = CoinManager.Instance.TotalCoins.ToString();
        }
    }
}