using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic; // 必須！
using UnityEngine.UI;

public class Skilltree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("スキルデータ")]

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
        if (UnlokkedSkill)//解放済みの色
        {
            ButtonImage.color = Color.yellow;
        }
        else if (AvailobleSkill)//開放可能
        {
            ButtonImage.color = Color.red;//new Color(1f, 0.9f, 0.2f);
        }
    }
}

