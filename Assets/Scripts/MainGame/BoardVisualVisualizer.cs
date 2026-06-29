using UnityEngine;

public class BoardVisualVisualizer : MonoBehaviour
{
    [Header("初期サイズ用の背景・縁セット（bonusBoardSize == 0）")]
    [SerializeField] private GameObject defaultBoardVisual;

    [Header("拡張サイズ用の背景・縁セット（bonusBoardSize > 0）")]
    [SerializeField] private GameObject expandedBoardVisual;

    void Start()
    {
        ApplyBoardVisual();
    }

    /// <summary>
    /// BoardSizeManagerのデータに基づいて見た目を切り替える
    /// </summary>
    public void ApplyBoardVisual()
    {
        // マネージャーが存在しない場合は初期状態にしておく
        if (BoardSizeManager.Instance == null)
        {
            SetVisualStates(isExpanded: false);
            return;
        }

        // bonusBoardSize が 1 以上なら拡張後、0 なら初期状態
        bool isExpanded = BoardSizeManager.Instance.bonusBoardSize > 0;
        SetVisualStates(isExpanded);
    }

    private void SetVisualStates(bool isExpanded)
    {
        if (defaultBoardVisual != null)
        {
            defaultBoardVisual.SetActive(!isExpanded);
        }

        if (expandedBoardVisual != null)
        {
            expandedBoardVisual.SetActive(isExpanded);
        }
    }
}