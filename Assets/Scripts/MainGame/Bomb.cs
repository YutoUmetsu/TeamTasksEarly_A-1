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

        // ゲーム画面内の見た目を強化版に切り替える処理
        TryUpgradeInGameSprite();
    }

    // MakeSwitch.cs から呼ばれる起爆用の窓口関数
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

        // ─── ★【超重要：処理順の修正】★ ───
        // 爆風を安全にアクティブ化する前に親を切ると、FallBomb+側が管理クラスに瞬殺され、
        // 2つ目以降の爆風オブジェクトがこの世から消滅（巻き添え削除）してしまいます。
        // なので、まずは「すべてアクティブ化」してから独立させます。
        if (explosionAreaGroup != null)
        {
            ExplosionHit[] hitScripts = explosionAreaGroup.GetComponentsInChildren<ExplosionHit>(true);
            foreach (ExplosionHit hitScript in hitScripts)
            {
                hitScript.SetDamage(totalDamage);
            }

            // 先に全方向の爆風オブジェクトのセットを画面に出す
            explosionAreaGroup.SetActive(true);

            // 【画像救出】SetDelete() の巻き添えで非表示になった爆風内の全画像をパッとオンに戻す
            SpriteRenderer[] childSprites = explosionAreaGroup.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in childSprites)
            {
                if (sr.gameObject != this.gameObject)
                {
                    sr.enabled = true;
                }
            }

            // 爆風が時間経過で消えるためのコルーチンをキック
            StartCoroutine(DisableCollidersDelayed());
        }

        // 基本の爆風セットをすべて出し切った、安全な「ここ」で親子の縁を切る（独立させる）
        transform.SetParent(null);
        // ─────────────────────────────────────

        // 自分自身の見た目（画像）を消す
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
        float elapsed = 0f;

        // 爆風が出ている時間（destroyDelay秒）が経過するまで、毎フレーム画像を維持し続ける
        while (elapsed < destroyDelay)
        {
            if (explosionAreaGroup != null)
            {
                // 他のスクリプト（FallObjectなど）による非表示化を毎フレーム上書きして拒否する
                SpriteRenderer[] childSprites = explosionAreaGroup.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (SpriteRenderer sr in childSprites)
                {
                    if (sr.gameObject != this.gameObject)
                    {
                        sr.enabled = true;
                    }
                }
            }

            elapsed += Time.deltaTime;
            yield return null; // 1フレーム待つ
        }

        // 時間が来たら、爆風オブジェクト自体を完全に非アクティブにして消し去る
        if (explosionAreaGroup != null)
        {
            explosionAreaGroup.SetActive(false);
        }
    }
}