using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic; // 必須！

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
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

