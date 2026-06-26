using UnityEngine;
using TMPro;

public class ResultCoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ResCoinText;

    void Start()
    {
        // 画面が開いた時に、CoinManagerから増加量を1回だけ取得して表示
        if (ResCoinText != null && CoinManager.Instance != null)
        {
            ResCoinText.text =CoinManager.Instance.AddCoins.ToString();
            //コインの増加量を0にリセット
            CoinManager.Instance.AddCoins = 0;
        }
    }
}
