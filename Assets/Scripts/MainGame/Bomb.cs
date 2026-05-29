using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("判定まとめ用空オブジェクトをセット")]
    public GameObject explosionAreaGroup;

    [Header("爆発演出の持続時間")]
    public float destroyDelay = 0.5f;

    [Header("この爆弾がUIのストックに並ぶ時の画像")]
    public Sprite uiSprite;

    // ─── 【新機能：基礎攻撃力の設定】 ───
    [Header("爆弾の基礎攻撃力")]
    [SerializeField] private int baseDamage = 1;

    void Start()
    {
        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(false);
        }
    }

    public void Explode()
    {
        // ─── 【重要：シングルトンと合算して総火力を計算】 ───
        int totalDamage = baseDamage;
        if (BombPowerManager.Instance != null)
        {
            totalDamage += BombPowerManager.Instance.bonusDamage;
        }
        Debug.Log($"ドカーン！起爆しました（総攻撃力: {totalDamage}）");

        Controller controller = UnityEngine.Object.FindFirstObjectByType<Controller>();
        FallObject parentFallObj = GetComponentInParent<FallObject>();

        // 爆弾ブロック自身が身にまとっている FallObject もダメージではなく即死（SetDelete）させるか、
        // あるいはこれ自体も1つのブロックとして巻き込むなら TakeDamage(totalDamage) にします。
        // 今回は安全のため、爆弾自身は即座に消去フラグを立てます。
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
            // ─── 【新機能：爆風のコライダーたちにダメージを伝える】 ───
            ExplosionHit[] hitScripts = explosionAreaGroup.GetComponentsInChildren<ExplosionHit>(true);
            foreach (ExplosionHit hitScript in hitScripts)
            {
                hitScript.SetDamage(totalDamage); // 爆風に攻撃力を設定
            }

            explosionAreaGroup.SetActive(true);
            StartCoroutine(DisableCollidersDelayed());
        }

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        Destroy(gameObject, destroyDelay);
    }

    private System.Collections.IEnumerator DisableCollidersDelayed()
    {
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