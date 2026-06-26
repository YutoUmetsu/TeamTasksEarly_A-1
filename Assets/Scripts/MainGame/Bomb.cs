using UnityEngine;
using System.Collections.Generic;

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

    [Header("フェードインにかかる時間の割合 (0.0〜1.0)")]
    [Range(0f, 1f)]
    [SerializeField] private float fadeInRatio = 0.2f;

    [Header("フェードアウトにかかる時間の割合 (0.0〜1.0)")]
    [Range(0f, 1f)]
    [SerializeField] private float fadeOutRatio = 0.5f;

    [Header("通常時のUIストック画像")]
    public Sprite uiSprite;

    [Header("爆風強化が両方（T1・T2）終わっている時のUI画像")]
    [SerializeField] private Sprite upgradedUiSprite;

    [Header("見た目（画像）が変わる子オブジェクト")]
    [SerializeField] private GameObject spriteChildObject;

    [Header("爆風強化が両方終わっている時のゲーム画面用画像")]
    [SerializeField] private Sprite upgradedInGameSprite;

    [Header("爆弾の基礎攻撃力")]
    [SerializeField] private int baseDamage = 1;

    [Header("プレイヤーが設置した爆弾かどうか")]
    public bool isPlayerPlaced = false;

    void Start()
    {
        if (explosionAreaGroup != null) explosionAreaGroup.SetActive(false);
        if (extraBlastT1 != null) extraBlastT1.SetActive(false);
        if (extraBlastT2 != null) extraBlastT2.SetActive(false);

        TryUpgradeInGameSprite();
    }

    public void ExplodeBySwitch()
    {
        if (!isPlayerPlaced) return;
        Explode();
    }

    private void TryUpgradeInGameSprite()
    {
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
                    t2 = BlastAddManager.Instance.bombVarticle.hasBlastT2; // 修正箇所（タイポ除去）
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

        if (explosionAreaGroup != null)
        {
            ExplosionHit[] hitScripts = explosionAreaGroup.GetComponentsInChildren<ExplosionHit>(true);
            foreach (ExplosionHit hitScript in hitScripts)
            {
                hitScript.SetDamage(totalDamage);
            }

            explosionAreaGroup.SetActive(true);

            SpriteRenderer[] childSprites = explosionAreaGroup.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in childSprites)
            {
                if (sr.gameObject != this.gameObject)
                {
                    sr.enabled = true;

                    Color c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                }
            }

            StartCoroutine(DisableCollidersDelayed());
        }

        transform.SetParent(null);

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        if (spriteChildObject != null)
        {
            SpriteRenderer childSprite = spriteChildObject.GetComponent<SpriteRenderer>();
            if (childSprite != null) childSprite.enabled = false;
        }

        Destroy(gameObject, destroyDelay);
    }

    private System.Collections.IEnumerator DisableCollidersDelayed()
    {
        if (explosionAreaGroup == null) yield break;

        SpriteRenderer[] childSprites = explosionAreaGroup.GetComponentsInChildren<SpriteRenderer>(true);

        List<SpriteRenderer> blastSprites = new List<SpriteRenderer>();
        foreach (SpriteRenderer sr in childSprites)
        {
            if (sr.gameObject != this.gameObject) blastSprites.Add(sr);
        }

        if (blastSprites.Count == 0) yield break;

        float elapsed = 0f;

        while (elapsed < destroyDelay)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / destroyDelay);

            float targetAlpha = 1f;

            if (normalizedTime < fadeInRatio && fadeInRatio > 0)
            {
                targetAlpha = normalizedTime / fadeInRatio;
            }
            else if (normalizedTime > (1f - fadeOutRatio) && fadeOutRatio > 0)
            {
                float fadeOutStart = 1f - fadeOutRatio;
                targetAlpha = 1f - ((normalizedTime - fadeOutStart) / fadeOutRatio);
            }

            targetAlpha = Mathf.Clamp01(targetAlpha);

            foreach (SpriteRenderer sr in blastSprites)
            {
                if (sr == null) continue;

                sr.enabled = true;

                Color c = sr.color;
                c.a = targetAlpha;
                sr.color = c;
            }

            yield return null;
        }

        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(false);
        }
    }
}