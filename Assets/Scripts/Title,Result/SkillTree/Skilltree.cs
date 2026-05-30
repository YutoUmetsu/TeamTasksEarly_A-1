using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic; // 必須！
using UnityEngine.UI;

public class Skilltree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("スキルデータ")]

    public SkillType skillType;
    public string SkillName;
    public bool UnlokkedSkill; //取得済みスキル
    public bool AvailobleSkill = false;//取得可能スキル

    public int Cost = 1; //コスト追加
    public List<Skilltree> NextSkill;//次のスキル
    [SerializeField] SkillTreeManager manager;

    Image ButtonImage;


    //void Initializetree(Skilltree center) //中心のスキルのみ取得可能
    //{
    //    center.AvailobleSkill = true;  
    //}

    public void UnlockSkilltree(SkillTreeManager manager)
    {
       /* if (CoinManager.Instance.TrySpendCoins(Cost)) {
            switch (skillType)
            {
                // 名前が「Damage」から始まっている場合の処理（T1～T7共通）
                case var _ when skillType.ToString().StartsWith("Damage"):
                    Debug.Log("ダメージ系のスキルが解放されました！");
                    // ここにダメージアップの共通処理を書く
                    break;

                //レンジだけは個別に1個ずつ書く
                case SkillType.RangeT1:
                    Debug.Log("範囲T1のスキルが解放されました！");
                    break;
                case SkillType.RangeT2:
                    Debug.Log("範囲T2のスキルが解放されました！");
                    break;
                case SkillType.RangeT3:
                    Debug.Log("範囲T3のスキルが解放されました！");
                    break;
                case SkillType.RangeT4:
                    Debug.Log("範囲T4のスキルが解放されました！");
                    break;
                case SkillType.RangeT5:
                    Debug.Log("範囲T5のスキルが解放されました！");
                    break;
                case SkillType.RangeT6:
                    Debug.Log("範囲T6のスキルが解放されました！");
                    break;
                case SkillType.RangeT7:
                    Debug.Log("範囲T7のスキルが解放されました！");
                    break;

                // 名前が「Result」から始まっている場合の処理（T1～T7共通）
                case var _ when skillType.ToString().StartsWith("Result"):
                    Debug.Log("リザルト系のスキルが解放されました！");
                    break;

                //名前が「Combo」から始まっている場合の処理（T1～T7共通）
                case var _ when skillType.ToString().StartsWith("Combo"):
                    Debug.Log("コンボ系のスキルが解放されました！");
                    break;

                // どれにも当てはまらない場合（エラー除け）
                default:
                    Debug.Log("設定されていない、あるいは用意されてません！");
                    break;
            }*/


            if (UnlokkedSkill) return; //すでに取得済み

        if (!AvailobleSkill) return;  //取得不可

        if(manager.CostPoint < Cost) return; //ポイント不足

        manager.CostPoint -= Cost; //ポイント消費

        UnlokkedSkill = true;
        AvailobleSkill = false;

        //次のスキル開放
        foreach(var next in NextSkill)
        {
            next.AvailobleSkill = true;
        }


        manager.UpdatePointText(); //UI更新

        Debug.Log(SkillName + " を取得");


    }





    void Start()
    {
        ButtonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UnlokkedSkill&&!AvailobleSkill)
        {
            ButtonImage.color = new Color(0.4f, 0.4f, 0.4f);
        }
        else if (UnlokkedSkill)//解放済みの色
        {
            ButtonImage.color = Color.yellow;
        }
        else if (AvailobleSkill)//開放可能
        {
            ButtonImage.color = Color.red;//new Color(1f, 0.9f, 0.2f);
        }
    }

    //スキルツリーメーカー参照
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

