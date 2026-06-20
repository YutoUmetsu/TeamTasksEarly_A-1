using UnityEngine;

public enum SkillType
{
    //AboveSkill
    ExprosionPower11,//爆発威力アップ
    ResourceAmount11,//獲得リソースアップ
    ExprosionPower12,
    ExprosionHeightRange11,//「縦」爆発範囲増加
    BombAppearanceRate11,//爆発出現率
    ExprosionWidthRange11,//「横」爆発範囲増加
    ExprosionXRange11,//「×」爆発範囲増加
                      //bool
    ExprosionCrossRange11,//「十字」爆発範囲増加

    //UnderSkill
    ExprosionPower21,
    ResourceAmount21,
    ExprosionPowee22,
    ExprosionHeightRange21,
    BombAppearanceRate21,
    ExprosionWidthRange21,
    ExprosionXRange21,
    //bool
    ExprosionCrossRange21,

    //RightSkill
    ExprosionWidthRange31,
    ResourceAmount31,
    ExprosionPower31,
    ExprosionHeightRange31,
    BombAppearanceRate31,
    ExprosionWidthRange32,
    ExprosionXRange31,
    StartBomb31,//初期保有爆弾数
    ExprosionCrossRange31,

    //LeftSkill
    ExprosionHeightRange41,
    ResourceAmount41,
    ExprosionPower41,
    ExprosionHeightRange42,
    BombAppearanceRate41,
    ExprosionWidthRange42,
    ExprosionXRange41,
    StartBomb41,
    ExprosionCrossRange41
}
