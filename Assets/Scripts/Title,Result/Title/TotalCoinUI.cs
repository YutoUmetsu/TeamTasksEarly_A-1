using UnityEngine;
using TMPro;

public class TotalCoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalCoinText;

    void Start()
    {
        // ‰æ–Ê‚ªŠJ‚¢‚½‚ÉACoinManager‚©‚ç‘—Ê‚ğ1‰ñ‚¾‚¯æ“¾‚µ‚Ä•\¦
        if (totalCoinText != null && CoinManager.Instance != null)
        {
            totalCoinText.text = CoinManager.Instance.TotalCoins.ToString();
        }
    }
}