using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ExplosionSwitchVisualManager : MonoBehaviour
{
    [System.Serializable]
    public struct SwitchSpriteGroup
    {
        // 0番目に3-3、1番目に3-2、2番目に3-1の画像を順番に設定してください
        public List<Sprite> clickSprites;
    }

    public List<SwitchSpriteGroup> switchPatternTable;
    [SerializeField] private Image switchButtonImage;

    private BombSet bombSet;

    void OnEnable()
    {
        if (bombSet == null)
        {
            bombSet = UnityEngine.Object.FindFirstObjectByType<BombSet>();
        }

        if (switchButtonImage == null)
        {
            switchButtonImage = GetComponent<Image>();
        }

        if (bombSet != null)
        {
            UpdateSwitchVisual();
        }
    }

    // メインの処理
    public void UpdateSwitchVisual()
    {
        if (bombSet == null || switchButtonImage == null) return;
        if (switchPatternTable == null || switchPatternTable.Count < 3) return;

        int maxBombs = bombSet.maxPlaceableBombs;
        if (BombInventoryManager.Instance != null)
        {
            maxBombs += BombInventoryManager.Instance.bonusMaxBombs;
        }

        int rowIndex = maxBombs - 3;
        rowIndex = Mathf.Clamp(rowIndex, 0, switchPatternTable.Count - 1);

        List<Sprite> currentSprites = switchPatternTable[rowIndex].clickSprites;
        if (currentSprites == null || currentSprites.Count == 0) return;

        // 【修正点】設置が完了して増えた総数から「1」を引いてインデックスを割り出します
        // 1個設置時（totalPlacedCount = 1）：1 - 1 = 0番目（3-3）
        // 2個設置時（totalPlacedCount = 2）：2 - 1 = 1番目（3-2）
        // 3個設置時（totalPlacedCount = 3）：3 - 1 = 2番目（3-1）
        int spriteIndex = bombSet.TotalPlacedCount - 1;
        spriteIndex = Mathf.Clamp(spriteIndex, 0, currentSprites.Count - 1);

        switchButtonImage.sprite = currentSprites[spriteIndex];
    }

    // エラー回避用のダミー関数
    public void UpdateSwitchVisual(int dummyUnused)
    {
        UpdateSwitchVisual();
    }
}