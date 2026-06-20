using UnityEngine;

public class BlinkStars : MonoBehaviour
{
    [SerializeField] float blinkInterval = 0.5f; // 点滅間隔
    [SerializeField] float rotateAngle = 15f;    // 1回ごとの回転角度

    SpriteRenderer sr;

    bool visible = true;
    float timer = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= blinkInterval)
        {
            timer = 0f;

            // 表示・非表示切り替え
            visible = !visible;

            Color color = sr.color;
            color.a = visible ? 1f : 0f;
            sr.color = color;

            // 点滅のたびに回転
            transform.Rotate(0f, 0f, rotateAngle);
        }
    }
}
