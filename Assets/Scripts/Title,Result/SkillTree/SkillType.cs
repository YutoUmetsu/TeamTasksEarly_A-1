using UnityEngine;

public enum SkillType
{
  //AboveSkill
    bool ExprosionPower11;//爆発威力アップ
    bool ResourceAmount11;//獲得リソースアップ
    bool ExprosionPower12;
    bool ExprosionHeightRange11;//「縦」爆発範囲増加
    bool BombAppearanceRate11;//爆発出現率
    bool ExprosionWidthRange11;//「横」爆発範囲増加
    bool ExprosionXRange11;//「×」爆発範囲増加
    //bool
    bool ExprosionCrossRange11;//「十字」爆発範囲増加

    //UnderSkill
    bool ExprosionPower21;
    bool ResourceAmount21;
    bool ExprosionPowee22;
    bool ExprosionHeightRange21;
    bool BombAppearanceRate21;
    bool ExprosionWidthRange21;
    bool ExprosionXRange21;
    //bool
    bool ExprosionCrossRange21;

    //RightSkill
    bool ExprosionWidthRange31;
    bool ResourceAmount31;
    bool ExprosionPower31;
    bool ExprosionHeightRange31;
    bool BombAppearanceRate31;
    bool ExprosionWidthRange32;
    bool ExprosionXRange31;
    bool StartBomb31;//初期保有爆弾数
    bool ExprosionCrossRange31;

    //LeftSkill
    bool ExprosionHeightRange41;
    bool ResourceAmount41;
    bool ExprosionPower41;
    bool ExprosionHeightRange42;
    bool BombAppearanceRate41;
    bool ExprosionWidthRange42;
    bool ExprosionXRange41;
    bool StartBomb41;
    bool ExprosionCrossRange41;
}