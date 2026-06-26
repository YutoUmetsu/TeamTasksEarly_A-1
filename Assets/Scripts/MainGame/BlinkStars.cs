using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]

public class BlinkStars : MonoBehaviour
{
    private float fadeTime = 1.0f;   // フェード時間
    private float minAlpha = 0.2f;   // 一番薄い透明度
    private float maxAlpha = 1.0f;   // 一番濃い透明度

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            // 薄くなる
            yield return Fade(maxAlpha, minAlpha);

            // 左右反転
            spriteRenderer.flipX = !spriteRenderer.flipX;

            // 濃くなる
            yield return Fade(minAlpha, maxAlpha);
        }
    }

    IEnumerator Fade(float start, float end)
    {
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, time / fadeTime);

            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            yield return null;
        }

        Color c = spriteRenderer.color;
        c.a = end;
        spriteRenderer.color = c;
    }
}
