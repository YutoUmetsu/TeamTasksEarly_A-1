using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic; // 必須！

[Serializable]


public class Skilltree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //[Header("スキルデータ")]

    public string SkillName;
    public bool Unlokkedskill; //取得済みスキル
    public bool Availobleskill = false;//取得可能スキル
    public List<Skilltree> NextSkill;//次のスキル


    void Initializetree(Skilltree center) //中心のスキルのみ取得可能
    {
        center.Availobleskill = true;  
    }

    public void UnlockSkilltree(Skilltree Tree)
    {
        if (!Tree.Availobleskill) return;

        Tree.Unlokkedskill = true;
        Tree.Availobleskill = false;

        //次のスキル開放
        foreach(var next in Tree.NextSkill)
        {
            next.Availobleskill = true;
        }
    }





    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class SkillTreeManager : MonoBehaviour
{
    public int CostPoint = 0;

    public TMP_Text pointsText;

    void Updeate()
    {
        pointsText.text = "ポイント:" + CostPoint;
    }
}