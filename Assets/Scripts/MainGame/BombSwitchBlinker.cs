using UnityEngine;
using UnityEngine.UI;

public class BombSwitchBlinker : MonoBehaviour
{
    private Image targetImage;

    [Header("点滅のスピード")]
    public float blinkSpeed = 3.0f;

    [Header("グレー時の色の濃さ (0が真っ黒、1が通常色。0.5前後がおすすめ)")]
    [Range(0f, 1f)]
    public float maxGrayDepth = 0.5f;

    void Awake()
    {
        targetImage = GetComponent<Image>();
    }

    // オブジェクトがアクティブ（表示）になった瞬間に自動で呼ばれる
    void OnEnable()
    {
        // 表示されたときは確実に元の色（白・通常）からスタートさせる
        if (targetImage != null)
        {
            targetImage.color = Color.white;
        }
    }

    void Update()
    {
        if (targetImage == null) return;

        // 0.0 から 1.0 の間をなめらかに行ったり来たりする値を作る
        float pingPong = Mathf.PingPong(Time.time * blinkSpeed, 1.0f);

        // 完全に真っ黒（0）にならないよう、maxGrayDepth で下限を制限する
        float colorMultiplier = Mathf.Lerp(maxGrayDepth, 1.0f, pingPong);

        // ★重要：ExplosionSwitchVisualManagerが画像を切り替えてもバグらないように、
        // Color.white(1, 1, 1) を基準にRGBをグレーに落とし込む（アルファ値は1を維持）
        targetImage.color = new Color(
            colorMultiplier,
            colorMultiplier,
            colorMultiplier,
            1.0f
        );
    }

    // オブジェクトが非アクティブ（非表示）になった瞬間に自動で呼ばれる
    void OnDisable()
    {
        // 非表示になるときは色を完全にリセットしておく
        if (targetImage != null)
        {
            targetImage.color = Color.white;
        }
    }
}