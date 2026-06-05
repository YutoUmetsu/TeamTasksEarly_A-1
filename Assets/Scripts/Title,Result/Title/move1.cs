using UnityEngine;
using DG.Tweening;

public class move1 : MonoBehaviour
{
    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();

        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPos(new Vector2(-425f, 260f), 0.2f)); // è„Ç…éùÇøè„Ç∞
        seq.Append(rt.DOAnchorPos(new Vector2(-425f, 210f), 0.2f)); // ñﬂÇ∑
        seq.Append(rt.DOAnchorPos(new Vector2(-425f, 240f), 0.15f));
        seq.Append(rt.DOAnchorPos(new Vector2(-425f, 210f), 0.15f));

        seq.Append(rt.DOAnchorPos(new Vector2(-425f, 210f), 3.3f)); // ç≈èIà⁄ìÆ
    }
}