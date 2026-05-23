using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("判定まとめ用空オブジェクトをセット")]
    public GameObject explosionAreaGroup;

    [Header("爆発演出の持続時間")]
    public float destroyDelay = 0.5f;

    [Header("この爆弾がUIのストックに並ぶ時の画像")]
    public Sprite uiSprite;

    void Start()
    {
        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(false);
        }
    }

    public void Explode()
    {
        Debug.Log("ドカーン！起爆しました");

        Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
        FallObject parentFallObj = GetComponentInParent<FallObject>();

        if (parentFallObj != null)
        {
            parentFallObj.SetDelete();
        }

        if (controller != null)
        {
            controller.TriggerExplosionFall();
        }

        transform.SetParent(null);

        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(true);
            StartCoroutine(DisableCollidersDelayed());
        }

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        Destroy(gameObject, destroyDelay);
    }

    // 物理演算が確実に1回計算されるのを待ってから判定をオフにする
    private System.Collections.IEnumerator DisableCollidersDelayed()
    {
        // 通常の1フレーム待機（null）ではなく、物理演算の更新（FixedUpdate）を確実に1回待つ
        yield return new WaitForFixedUpdate();

        if (explosionAreaGroup != null)
        {
            Collider2D[] explosionColliders = explosionAreaGroup.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in explosionColliders)
            {
                if (col != null) col.enabled = false;
            }
        }
    }
}