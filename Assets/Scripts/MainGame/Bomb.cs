using UnityEngine;

public class Bomb : MonoBehaviour
{
    public enum BombType
    {
        Vertical,
        Horizon,
        CrossT,
        CrossX
    }

    [Header("─── スキル連動設定 ───")]
    [Header("この爆弾の種類")]
    public BombType bombType;

    [Header("追加爆風のオブジェクト（Tier1用、Tier2用）")]
    [SerializeField] private GameObject extraBlastT1;
    [SerializeField] private GameObject extraBlastT2;

    [Header("判定まとめ用空オブジェクトをセット")]
    public GameObject explosionAreaGroup;

    [Header("爆発演出の持続時間")]
    public float destroyDelay = 0.5f;

    [Header("通常時のUIストック画像")]
    public Sprite uiSprite;

    [Header("爆風強化が両方（T1・T2）終わっている時のUI画像")]
    [SerializeField] private Sprite upgradedUiSprite;

    // ─── 【新機能：ゲーム画面内の見た目チェンジ用】 ───
    [Header("見た目（画像）が変わる子オブジェクト")]
    [SerializeField] private GameObject spriteChildObject;

    [Header("爆風強化が両方終わっている時のゲーム画面用画像")]
    [SerializeField] private Sprite upgradedInGameSprite;
    // ──────────────────────────────────────────

    [Header("爆弾の基礎攻撃力")]
    [SerializeField] private int baseDamage = 1;

    void Start()
    {
        if (explosionAreaGroup != null) explosionAreaGroup.SetActive(false);
        if (extraBlastT1 != null) extraBlastT1.SetActive(false);
        if (extraBlastT2 != null) extraBlastT2.SetActive(false);

        // 【新機能】ゲーム画面内の見た目を強化版に切り替える処理
        TryUpgradeInGameSprite();
    }

    // 新しく追加する関数：条件を満たしていれば配置された爆弾の見た目を変える
    private void TryUpgradeInGameSprite()
    {
        // 強化後の画像と、見た目用の子オブジェクトがセットされていないなら何もしない（他の3種はスルーされる）
        if (upgradedInGameSprite == null || spriteChildObject == null) return;

        if (BlastAddManager.Instance != null)
        {
            bool t1AndT2Unlocked = false;

            switch (bombType)
            {
                case BombType.Vertical:
                    t1AndT2Unlocked = BlastAddManager.Instance.bombVarticle.hasBlastT1 && BlastAddManager.Instance.bombVarticle.hasBlastT2;
                    break;
                case BombType.Horizon:
                    t1AndT2Unlocked = BlastAddManager.Instance.bombHorizon.hasBlastT1 && BlastAddManager.Instance.bombHorizon.hasBlastT2;
                    break;
                case BombType.CrossT:
                    t1AndT2Unlocked = BlastAddManager.Instance.bombCrossT.hasBlastT1 && BlastAddManager.Instance.bombCrossT.hasBlastT2;
                    break;
                case BombType.CrossX:
                    t1AndT2Unlocked = BlastAddManager.Instance.bombCrossX.hasBlastT1 && BlastAddManager.Instance.bombCrossX.hasBlastT2;
                    break;
            }

            // 両方解放されていたら、子オブジェクトのSpriteRendererを書き換える！
            if (t1AndT2Unlocked)
            {
                SpriteRenderer childSpriteRenderer = spriteChildObject.GetComponent<SpriteRenderer>();
                if (childSpriteRenderer != null)
                {
                    childSpriteRenderer.sprite = upgradedInGameSprite;
                    Debug.Log($"{gameObject.name} のゲーム内見た目を強化版に変更しました！");
                }
            }
        }
    }

    public Sprite GetCurrentUiSprite()
    {
        if (BlastAddManager.Instance != null && upgradedUiSprite != null)
        {
            bool t1AndT2Unlocked = false;

            switch (bombType)
            {
                case BombType.Vertical:
                    t1AndT2Unlocked = BlastAddManager.Instance.bombVarticle.hasBlastT1 && BlastAddManager.Instance.bombVarticle.hasBlastT2;
                    break;
                case BombType.Horizon:
                    t1AndT2Unlocked = BlastAddManager.Instance.bombHorizon.hasBlastT1 && BlastAddManager.Instance.bombHorizon.hasBlastT2;
                    break;
                case BombType.CrossT:
                    t1AndT2Unlocked = BlastAddManager.Instance.bombCrossT.hasBlastT1 && BlastAddManager.Instance.bombCrossT.hasBlastT2;
                    break;
                case BombType.CrossX:
                    t1AndT2Unlocked = BlastAddManager.Instance.bombCrossX.hasBlastT1 && BlastAddManager.Instance.bombCrossX.hasBlastT2;
                    break;
            }

            if (t1AndT2Unlocked)
            {
                return upgradedUiSprite;
            }
        }

        return uiSprite;
    }

    public void Explode()
    {
        int totalDamage = baseDamage;
        if (BombPowerManager.Instance != null)
        {
            totalDamage += BombPowerManager.Instance.bonusDamage;
        }
        Debug.Log($"ドカーン！起爆しました（総攻撃力: {totalDamage}）");

        if (BlastAddManager.Instance != null)
        {
            bool t1 = false;
            bool t2 = false;

            switch (bombType)
            {
                case BombType.Vertical:
                    t1 = BlastAddManager.Instance.bombVarticle.hasBlastT1;
                    t2 = BlastAddManager.Instance.bombVarticle.hasBlastT2;
                    break;
                case BombType.Horizon:
                    t1 = BlastAddManager.Instance.bombHorizon.hasBlastT1;
                    t2 = BlastAddManager.Instance.bombHorizon.hasBlastT2;
                    break;
                case BombType.CrossT:
                    t1 = BlastAddManager.Instance.bombCrossT.hasBlastT1;
                    t2 = BlastAddManager.Instance.bombCrossT.hasBlastT2;
                    break;
                case BombType.CrossX:
                    t1 = BlastAddManager.Instance.bombCrossX.hasBlastT1;
                    t2 = BlastAddManager.Instance.bombCrossX.hasBlastT2;
                    break;
            }

            if (t1 && extraBlastT1 != null) extraBlastT1.SetActive(true);
            if (t2 && extraBlastT2 != null) extraBlastT2.SetActive(true);
        }

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
            ExplosionHit[] hitScripts = explosionAreaGroup.GetComponentsInChildren<ExplosionHit>(true);
            foreach (ExplosionHit hitScript in hitScripts)
            {
                hitScript.SetDamage(totalDamage);
            }

            explosionAreaGroup.SetActive(true);
            StartCoroutine(DisableCollidersDelayed());
        }

        // ─── 【修正：子オブジェクトの画像も一緒に非表示にする】 ───
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        if (spriteChildObject != null)
        {
            SpriteRenderer childSprite = spriteChildObject.GetComponent<SpriteRenderer>();
            if (childSprite != null) childSprite.enabled = false;
        }
        // ────────────────────────────────────────────────────────

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