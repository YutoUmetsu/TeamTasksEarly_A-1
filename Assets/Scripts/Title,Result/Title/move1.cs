using UnityEngine;
using DG.Tweening;

public class Move1 : MonoBehaviour
{
    public static bool played = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (played) 
        {
            transform.position = new Vector3(525.0f, 740.0f, 0.0f);
            return;
        }
        played = true;
        PlayAnimation();
        //transform.DOMove(new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
    }

   void PlayAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveX(525.0f, 2.2f));
        seq.Join(transform.DOMoveY(740.0f, 2.2f).SetEase(Ease.OutBounce));
        seq.Join(transform.DOMoveZ(0.0f, 2.2f));

        seq.Play();

    }
}
