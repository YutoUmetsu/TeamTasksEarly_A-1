using UnityEngine;
using DG.Tweening;

public class Move2 : MonoBehaviour
{
    public static bool played = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (played)
        {
            transform.position = new Vector3(1400.0f, transform.position.y, transform.position.z);
            return;
        }
            played = true;
        PlayAnimation();

    }

    void PlayAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(transform.DOMoveX(1400.0f, 0.5f)
            .SetDelay(3.0f)
            .SetEase(Ease.OutExpo));

        seq.Join(transform.DOScale(
            new Vector3(0.8f, 1.2f, 1f),
            0.5f));

        seq.Append(transform.DOScale(
            new Vector3(1.2f, 0.8f, 1f),
            0.08f));

        seq.Append(transform.DOScale(
            Vector3.one,
            0.12f).SetEase(Ease.OutBack));

        seq.Play();

    }
}
