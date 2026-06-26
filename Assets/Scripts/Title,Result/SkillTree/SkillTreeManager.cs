using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("GameStarted"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("GameStarted", 1);
        }
    }

    public int CostPoint; // Startで初期化するため宣言のみ
    public Button PrestigeButton;
    public Skilltree CenterSkill;
    public TMP_Text pointsText;
    public List<Skilltree> AllSkills;

    [SerializeField] Skill skill;
    [SerializeField] SkillTree skilltree;

    // ─── スキル効果値の設定（マジックナンバー対策） ───
    private const int EXPLOSION_POWER_BONUS = 1;         // 爆発威力アップの値
    private const int RESOURCE_COIN_BONUS = 5;         // コインボーナスアップの値
    private const float BOMB_SPAWN_PERCENT_BONUS = 5f;   // 爆弾出現率アップの値（%）
    private const int BOMB_INVENTORY_BONUS = 1;         // 初期保有（設置上限）アップの値
    private const int PRESTIGE_BOARD_SIZE_BONUS = 1;     // プレステージ時、1回あたりに広げる盤面サイズの値 ★追加

    public void ApplySkill(SkillType type) // 能力反映処理
    {
        switch (type)
        {
     

            // ─── 1. 爆発威力アップ（BombPowerManager） ───
            case SkillType.ExprosionPower11:
            case SkillType.ExprosionPower12:
            case SkillType.ExprosionPower21:
            case SkillType.ExprosionPower31:
            case SkillType.ExprosionPower41:
                if (BombPowerManager.Instance != null)
                    BombPowerManager.Instance.IncreaseDamage(EXPLOSION_POWER_BONUS);
                break;

            // ─── 2. 獲得リソース＝コインアップ（CoinBonusManager） ───
            case SkillType.ResourceAmount11:
            case SkillType.ResourceAmount21:
            case SkillType.ResourceAmount31:
            case SkillType.ResourceAmount41:
                if (CoinBonusManager.Instance != null)
                    CoinBonusManager.Instance.IncreaseCoinBonus(RESOURCE_COIN_BONUS);
                break;

            // ─── 3. 爆弾出現率アップ（BombSpawnManager） ───
            case SkillType.BombAppearanceRate11:
            case SkillType.BombAppearanceRate21:
            case SkillType.BombAppearanceRate31:
            case SkillType.BombAppearanceRate41:
                if (BombSpawnManager.Instance != null)
                    BombSpawnManager.Instance.IncreaseSpawnBombs(BOMB_SPAWN_PERCENT_BONUS);
                break;

            // ─── 4. 初期保有爆弾（設置上限）アップ（BombInventoryManager） ───
            case SkillType.StartBomb31:
            case SkillType.StartBomb41:
                if (BombInventoryManager.Instance != null)
                    BombInventoryManager.Instance.IncreaseMaxBombs(BOMB_INVENTORY_BONUS);
                break;

            // ─── 5. 「縦」爆発範囲（縦爆弾の追加爆風） ───
            case SkillType.ExprosionHeightRange11:
            case SkillType.ExprosionHeightRange21:
            case SkillType.ExprosionHeightRange31:
            case SkillType.ExprosionHeightRange41:
            case SkillType.ExprosionHeightRange42:
                if (BlastAddManager.Instance != null)
                {
                    if (!BlastAddManager.Instance.bombVarticle.hasBlastT1)
                        BlastAddManager.Instance.bombVarticle.hasBlastT1 = true;
                    else
                        BlastAddManager.Instance.bombVarticle.hasBlastT2 = true;
                }
                break;

            // ─── 6. 「横」爆発範囲（横爆弾の追加爆風） ───
            case SkillType.ExprosionWidthRange11:
            case SkillType.ExprosionWidthRange21:
            case SkillType.ExprosionWidthRange31:
            case SkillType.ExprosionWidthRange32:
            case SkillType.ExprosionWidthRange42:
                if (BlastAddManager.Instance != null)
                {
                    if (!BlastAddManager.Instance.bombHorizon.hasBlastT1)
                        BlastAddManager.Instance.bombHorizon.hasBlastT1 = true;
                    else
                        BlastAddManager.Instance.bombHorizon.hasBlastT2 = true;
                }
                break;

            // ─── 7. 「×」爆発範囲（クロスX爆弾の追加爆風） ───
            case SkillType.ExprosionXRange11:
            case SkillType.ExprosionXRange21:
            case SkillType.ExprosionXRange31:
            case SkillType.ExprosionXRange41:
                if (BlastAddManager.Instance != null)
                {
                    if (!BlastAddManager.Instance.bombCrossX.hasBlastT1)
                        BlastAddManager.Instance.bombCrossX.hasBlastT1 = true;
                    else
                        BlastAddManager.Instance.bombCrossX.hasBlastT2 = true;
                }
                break;

            // ─── 8. 「十字」爆発範囲（クロスT爆弾の追加爆風） ───
            case SkillType.ExprosionCrossRange11:
            case SkillType.ExprosionCrossRange21:
            case SkillType.ExprosionCrossRange31:
            case SkillType.ExprosionCrossRange41:
                if (BlastAddManager.Instance != null)
                {
                    if (!BlastAddManager.Instance.bombCrossT.hasBlastT1)
                        BlastAddManager.Instance.bombCrossT.hasBlastT1 = true;
                    else
                        BlastAddManager.Instance.bombCrossT.hasBlastT2 = true;
                }
                break;

            default:
                Debug.LogWarning($"{type} の効果反映処理が未設定です。");
                break;
        }
    }

    // 全取得チェック
    public bool IsAllSkillUnlocked()
    {
        foreach (var skill in AllSkills)
        {
            if (!skill.UnlokkedSkill)
            {
                return false;
            }
        }

        return true;
    }

    public void SaveData() // スキルツリー全体を保存
    {
        foreach (var skill in AllSkills)
        {
            skill.SaveSkill();
        }

        PlayerPrefs.SetInt("CostPoint", CostPoint);
        PlayerPrefs.Save();
    }

    public void LoadData() // スキルツリー全体を読み込み
    {
        foreach (var skill in AllSkills)
        {
            skill.LoadSkill();
            skill.AvailobleSkill = false;
        }
        foreach (var skill in AllSkills)
        {
            if (skill.UnlokkedSkill)
            {
                foreach (var next in skill.NextSkill)
                {
                    next.AvailobleSkill = true;
                }
            }
        }
        if (!CenterSkill.UnlokkedSkill)
        {
            CenterSkill.AvailobleSkill = true;
        }
        CenterSkill.AvailobleSkill = !CenterSkill.UnlokkedSkill;
        CostPoint = PlayerPrefs.GetInt("CostPoint", 0);
    }

    // プレステージ（リセット＆ステージ拡張処理）
    public void Prestige()
    {
        // 1. 各マネージャーの一般スキルボーナスをリセット
        if (BlastAddManager.Instance != null) BlastAddManager.Instance.ResetSkillData();
        if (BombInventoryManager.Instance != null) BombInventoryManager.Instance.ResetSkillData();
        if (BombPowerManager.Instance != null) BombPowerManager.Instance.ResetSkillData();
        if (BombSpawnManager.Instance != null) BombSpawnManager.Instance.ResetSkillData();
        if (CoinBonusManager.Instance != null) CoinBonusManager.Instance.ResetSkillData();

        // 2. プレステージボーナス：盤面サイズを拡張！
        if (BoardSizeManager.Instance != null)
        {
            BoardSizeManager.Instance.IncreaseBoardSize(PRESTIGE_BOARD_SIZE_BONUS);
        }

        // 3. スキルツリーのUIとデータの初期化
        foreach (var skill in AllSkills)
        {
            skill.UnlokkedSkill = false;
            skill.AvailobleSkill = false;
            skill.SetStartSprite();
        }

        CenterSkill.AvailobleSkill = true;

        Debug.Log("スキルツリーの解放状況をリセットしました。");

        // 4. データを保存してロードし直す
        SaveData();
        LoadData();

        CenterSkill.AvailobleSkill = !CenterSkill.UnlokkedSkill;

        // 5. コイン・ポイントUIの更新
        UpdatePointText();
    }

    void Start()
    {
        // 安全に初期値を取得（NullReferenceException対策）
        if (CoinManager.Instance != null)
        {
            CostPoint = CoinManager.Instance.TotalCoins;
        }

        Debug.Log(CenterSkill);

        if (CenterSkill == null)
        {
            Debug.LogError("CenterSkillが未設定");
            return;
        }

        if (pointsText == null)
        {
            Debug.LogError("pointsTextが未設定");
            return;
        }

        LoadData();

        if (!CenterSkill.UnlokkedSkill)
        {
            CenterSkill.AvailobleSkill = true;
        }

        UpdatePointText();
    }

    void Update()
    {
        if (IsAllSkillUnlocked())
        {
            PrestigeButton.interactable = true;
        }
        else
        {
            PrestigeButton.interactable = false;
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void UpdatePointText() // UI更新専用の関数
    {
        CostPoint = CoinManager.Instance.TotalCoins;
        pointsText.text = "ポイント:" + CostPoint;
    }
}